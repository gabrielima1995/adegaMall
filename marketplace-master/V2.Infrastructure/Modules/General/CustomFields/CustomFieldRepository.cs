using Dapper;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Framework;
using Infrastructure.Data.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using V2.Infrastructure.Data;

namespace V2.Infrastructure.Modules.General.CustomFields
{
    public class CustomFieldRepository : ICustomFieldRepository
    {
        private const string ValueQuery = @"SELECT * FROM CustomFieldValue WHERE EntityId = @entityId AND CustomField_Id = @customFieldId";

        private readonly ScaffoldContext _context;

        public CustomFieldRepository(ScaffoldContext context)
        {
            _context = context;
        }

        public OperationResult Add(CustomField entity)
        {
            _context.Set<CustomField>().Add(entity);
            return _context.SaveChanges() > 0 ? OperationResult.Success : OperationResult.Fail("Error adding entity to Repository");
        }

        public OperationResult Delete(Guid fieldId)
        {
            var entity = _context.Set<CustomField>().Include(x => x.Options).Include(x => x.Values).First(x => x.Id == fieldId);

            for (int i = 0; i < entity.Options.Count; i++)
                _context.Entry(entity.Options[i]).State = EntityState.Deleted;

            for (int i = 0; i < entity.Values.Count; i++)
                _context.Entry(entity.Values[i]).State = EntityState.Deleted;

            _context.SaveChanges();
            _context.Entry(entity).State = EntityState.Deleted;

            if (_context.SaveChanges() == 0)
                return OperationResult.Fail("Error adding entity to database.");

            return OperationResult.Success;
        }

        public OperationResult DeleteForEntityId(Guid entityId)
        {
            var entity = _context.Set<CustomFieldValue>().Where(x => x.EntityId == entityId).ToList();
            if (entity == null || entity.Count == 0)
                return OperationResult.Success;

            foreach (var item in entity)
            {
                _context.Entry(item).State = EntityState.Deleted;
            }

            return OperationResult.Success;
        }

        public OperationResult DeleteOption(CustomField entity, CustomFieldOption option)
        {
            entity.Options.Remove(option);

            _context.Entry(entity).State = EntityState.Modified;
            _context.Entry(option).State = EntityState.Deleted;

            return _context.SaveChanges() > 0 ? OperationResult.Success : OperationResult.Fail("Error deleting field option");
        }

        public Dictionary<Guid, CustomField> DictionaryByEntityType(CustomFieldEntityType entityType)
        {
            var list = ListByEntityType(entityType);
            var dict = new Dictionary<Guid, CustomField>();

            foreach (var item in list)
                dict.Add(item.Id, item);

            dict = dict.OrderBy(x => x.Value.Order).ToDictionary(pair => pair.Key, pair => pair.Value);
            return dict;
        }

        public CustomField GetById(string fieldId)
        {
            Guid id = new Guid(fieldId);
            return _context.CustomFields.Include(x => x.Options).First(x => x.Id == id);
        }

        public List<CustomFieldValue> GetCustomFieldValueByEntityId(string entityId)
        {
            Guid id = new Guid(entityId);
            return _context.CustomFieldValues.Where(x => x.EntityId == id).ToList();
        }

        public OperationResult UpdateCustomFieldValue(CustomFieldValue value)
        {
            var entry = _context.Entry(value);
            entry.State = EntityState.Modified;
            return _context.SaveChanges() >= 0 ? OperationResult.Success : OperationResult.Fail("Error updating entity");
        }

        public List<CustomField> ListByEntityType(CustomFieldEntityType entityType)
        {
            var list = _context.CustomFields.Include(x => x.Options).ToList();
            list = list.Where(x => x.EntityType == entityType).ToList();
            list = list.OrderBy(x => x.Order).ToList();
            return list;
        }

        public OperationResult SetValues(Guid entityId, Dictionary<string, string> values)
        {
            if (entityId == Guid.Empty)
                return OperationResult.Fail(nameof(entityId), "The entity ID must be specified.");

            if (values == null || values.Count == 0)
                return OperationResult.Fail(nameof(values), "The values must be specified.");

            var statements = new List<string>();
            var variables = new Dictionary<string, object>();

            for (int i = 0; i < values.Count; i++)
            {
                statements.Add($@"
                    UPDATE CustomFieldValue WITH (serializable)
                    SET Value = @value{i}
                    WHERE EntityId = @entityId{i} AND CustomField_Id = @fieldId{i}
                    IF @@rowcount = 0
                    BEGIN
                        INSERT CustomFieldValue (Id, EntityId, Value, CreatedAt, CustomField_Id)
                        VALUES (NEWID(), @entityId{i}, @value{i}, GETUTCDATE(), @fieldId{i})
                    END");

                var fieldId = values.Keys.ElementAt(i);
                var fieldValue = values[fieldId];

                variables.Add($"entityId{i}", entityId.ToString());
                variables.Add($"fieldId{i}", fieldId);
                variables.Add($"value{i}", fieldValue);
            }

            var sql = string.Join(Environment.NewLine, statements);

            using (var conn = MyDbConnection.Create())
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    object parameters = DynamicExtensions.DynamicFrom(variables);
                    conn.Execute(sql, parameters, tran);
                    tran.Commit();
                }
            }

            return OperationResult.Success;
        }

        public OperationResult SetValuesForAllQuotesInGroup(Guid groupId, Dictionary<string, string> fieldIdsAndValues)
        {
            if (groupId == Guid.Empty)
                throw new ArgumentException("The group id must be specified.");

            if (fieldIdsAndValues == null || fieldIdsAndValues.Count == 0)
                throw new ArgumentException("The fields and values must be specified.");

            using (var conn = MyDbConnection.Create())
            {
                var quoteIds = conn.Query<Guid>("SELECT Id FROM Quote WHERE GroupId = @groupId", new { groupId });

                var statements = new List<string>();
                var variables = new Dictionary<string, object>();
                var varCounter = 0;

                foreach (var quoteId in quoteIds)
                {
                    foreach (var fieldId in fieldIdsAndValues.Keys)
                    {
                        statements.Add($@"
                            UPDATE CustomFieldValue WITH (serializable)
                            SET Value = @value{varCounter}
                            WHERE EntityId = @entityId{varCounter} AND CustomField_Id = @fieldId{varCounter}
                            IF @@rowcount = 0
                            BEGIN
                                INSERT CustomFieldValue (Id, EntityId, Value, CreatedAt, CustomField_Id)
                                VALUES (NEWID(), @entityId{varCounter}, @value{varCounter}, GETUTCDATE(), @fieldId{varCounter})
                            END");

                        var fieldValue = fieldIdsAndValues[fieldId];

                        variables.Add($"entityId{varCounter}", quoteId.ToString());
                        variables.Add($"fieldId{varCounter}", fieldId);
                        variables.Add($"value{varCounter}", fieldValue);

                        varCounter++;
                    }
                }

                var sql = string.Join(Environment.NewLine, statements);

                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    object parameters = DynamicExtensions.DynamicFrom(variables);
                    conn.Execute(sql, parameters, tran);
                    tran.Commit();
                }
            }

            return OperationResult.Success;
        }

        public OperationResult Update(CustomField entity)
        {
            var entry = _context.Entry(entity);
            entry.State = EntityState.Modified;
            return _context.SaveChanges() >= 0 ? OperationResult.Success : OperationResult.Fail("Error updating entity");
        }

        public OperationResult CloneValues(Guid fromEntityId, Guid toEntityId)
        {
            using (var conn = MyDbConnection.Create())
            {
                var sql = @"
                    INSERT INTO CustomFieldValue
                    SELECT NEWID(), @toEntityId, [Value], GETUTCDATE(), CustomField_Id
                    FROM CustomFieldValue
                    WHERE EntityId = @fromEntityId";

                conn.Execute(sql, new { fromEntityId, toEntityId });

                return OperationResult.Success;
            }
        }
    }
}
