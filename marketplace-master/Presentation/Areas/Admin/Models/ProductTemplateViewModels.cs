using Application.Config;
using Application.Helpers;
using Domain.Entities.StoreModule;
using Domain.Modules.Products.Variants;
using Domain.Modules.ProductTemplateModule;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using V2.Application.Modules.ProductTemplateModule;

namespace Presentation.Areas.Admin.Models
{
    public class ProductTemplateCreateEditViewModel : ProductTemplateViewModel
    {
        public ProductTemplateCreateEditViewModel()
        {
        }

        public ProductTemplateCreateEditViewModel(ProductTemplate source) : base(source)
        {
        }

        public SelectList CategoryOptions { get; private set; }
        public void SetCategoryOptions(IEnumerable<ProductCategory> options) => CategoryOptions = new SelectList(options, nameof(ProductCategory.Id), nameof(ProductCategory.Name), CategoryId);

        public MultiSelectList VariantOptions { get; private set; }
        public void SetVariantOptions(IEnumerable<ProductVariantTypeAndItemDto> options)
        {
            if (options == null) return;
            var cast = options.Select(variant => new VariantSelectItem(variant));
            VariantOptions = new MultiSelectList(cast, nameof(VariantSelectItem.Id), nameof(VariantSelectItem.Name), nameof(VariantSelectItem.Group), Variants);
        }

        private class VariantSelectItem
        {
            public VariantSelectItem(int id)
            {
                Id = id;
            }
            public VariantSelectItem(ProductVariantTypeAndItemDto dto)
            {
                Id = dto.ItemId;
                Group = dto.TypeName;
                Name = $"{dto.TypeName}: {dto.ItemName}";
            }

            public int Id { get; }
            public string Group { get; }
            public string Name { get; }
        }
    }

    public class ProductTemplateDetailsViewModel : ProductTemplateViewModel
    {
        public ProductTemplateDetailsViewModel()
        {
        }

        public ProductTemplateDetailsViewModel(ProductTemplate source) : base(source)
        {
        }

        public List<object> Products { get; set; } = new List<object>();
        public List<object> Sellers { get; set; } = new List<object>();
    }

    public class ProductTemplateViewModel : IValidatableObject
    {
        public ProductTemplateViewModel()
        {
        }

        public ProductTemplateViewModel(ProductTemplate template)
        {
            if (template == null)
                return;

            Id = template.Id;
            Title = template.Title;
            TitleIsEditable = template.TitleIsEditable;
            CoverImageUrl = template.CoverImageUrl;
            CategoryId = template.Category.Id;
            CategoryIsEditable = template.CategoryIsEditable;
            Description = template.Description;
            DescriptionIsEditable = template.DescriptionIsEditable;
            Specification = template.Specification;
            SpecificationIsEditable = template.SpecificationIsEditable;
            Variants = template.Variants.SelectList(v => v.Id);
            VariantsIsEditable = template.VariantsIsEditable;
            IsEnabled = template.IsEnabled;
            Images = template.Images.SelectList(img => new ProductTemplateImageViewModel(img));
            ImagesIsEditable = template.ImagesIsEditable;
            Dimensions = ProductTemplateDimensionsViewModel.From(template.Dimensions, template.DimensionsIsEditable);
        }

        public Guid Id { get; set; } = Guid.NewGuid();

        [StringLengthLocalized(100)]
        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_TITLE")]
        public string Title { get; set; }

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_FIELD_IS_EDITABLE")]
        public bool TitleIsEditable { get; set; }

        public string CoverImageUrl { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_GATEGORY")]
        public int CategoryId { get; set; }

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_FIELD_IS_EDITABLE")]
        public bool CategoryIsEditable { get; set; }

        [StringLengthLocalized(8000)]
        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_DESCRIPTION")]
        public string Description { get; set; }

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_FIELD_IS_EDITABLE")]
        public bool DescriptionIsEditable { get; set; }

        [StringLengthLocalized(8000)]
        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_SPECIFICATION")]
        public string Specification { get; set; }

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_FIELD_IS_EDITABLE")]
        public bool SpecificationIsEditable { get; set; }

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_VARIANTS")]
        public List<int> Variants { get; set; } = new List<int>();

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_FIELD_IS_EDITABLE")]
        public bool VariantsIsEditable { get; set; }

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_IS_ENABLED")]
        public bool IsEnabled { get; set; } = true;

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_IMAGES")]
        public List<ProductTemplateImageViewModel> Images { get; set; } = new List<ProductTemplateImageViewModel>();

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_IMAGES_IS_EDITABLE")]
        public bool ImagesIsEditable { get; set; }

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_DIMENSIONS")]
        public ProductTemplateDimensionsViewModel Dimensions { get; set; } = new ProductTemplateDimensionsViewModel();

        public void CopyTo(ProductTemplate template)
        {
            template.Title = Title;
            template.TitleIsEditable = TitleIsEditable;

            template.CoverImageUrl = Images.FirstOrDefault()?.Url.ToString();

            template.CategoryId = CategoryId;
            template.CategoryIsEditable = CategoryIsEditable;

            template.Description = Description;
            template.DescriptionIsEditable = DescriptionIsEditable;

            template.Specification = Specification;
            template.SpecificationIsEditable = SpecificationIsEditable;

            template.Variants.Clear();
            template.Variants.AddRange(Variants.SelectList(v => new ProductVariant { Id = v }));
            template.VariantsIsEditable = VariantsIsEditable;

            template.IsEnabled = IsEnabled;

            var position = 1;
            template.Images.Clear();
            template.Images.AddRange(Images.SelectList(i => new ProductTemplateImage(i.Url.ToString(), position++)));
            template.ImagesIsEditable = ImagesIsEditable;

            Dimensions.CopyTo(template.Dimensions);
            template.DimensionsIsEditable = Dimensions.IsEditable;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Title) && !TitleIsEditable)
                yield return new ValidationResult("Se o título não for editável, ele deve ser preenchido.");

            if (string.IsNullOrWhiteSpace(Description) && !DescriptionIsEditable)
                yield return new ValidationResult("Se a descrição não for editável, ela deve ser preenchida.");

            if (string.IsNullOrWhiteSpace(Specification) && !SpecificationIsEditable)
                yield return new ValidationResult("Se as especificações técnicas não forem editáveis, elas devem ser preenchidas.");

            if (Variants.Count == 0 && MaaSConfig.Current.Product.Variants.Enabled && !VariantsIsEditable)
                yield return new ValidationResult("Se as características não forem editáveis, elas devem ser informadas.");

            if (Images.Count == 0 && !ImagesIsEditable)
                yield return new ValidationResult("Se as imagens não forem editáveis, envie pelo menos uma.");
        }
    }

    public class ProductTemplateDimensionsViewModel : IValidatableObject
    {
        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_DIMENSIONS_LENGTH")]
        public decimal? Length { get; set; }

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_DIMENSIONS_WIDTH")]
        public decimal? Width { get; set; }

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_DIMENSIONS_HEIGHT")]
        public decimal? Height { get; set; }

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_DIMENSIONS_WEIGHT")]
        public decimal? Weight { get; set; }

        [DisplayNameLocalized("PRODUCT_TEMPLATE_ADMIN_INPUT_FIELD_IS_EDITABLE")]
        public bool IsEditable { get; set; }

        public static ProductTemplateDimensionsViewModel From(ProductTemplateDimensions dimensions, bool dimensionsIsEditable)
        {
            return new ProductTemplateDimensionsViewModel
            {
                Length = dimensions.Length,
                Width = dimensions.Width,
                Height = dimensions.Height,
                Weight = dimensions.Weight,
                IsEditable = dimensionsIsEditable
            };
        }

        public void CopyTo(ProductTemplateDimensions dimensions)
        {
            dimensions.Length = Length;
            dimensions.Width = Width;
            dimensions.Height = Height;
            dimensions.Weight = Weight;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MaaSConfig.Current.Product.AnyDimensionsEnabled && !IsEditable && (
                Length == null || Length <= 0 ||
                Width == null || Width <= 0 ||
                Height == null || Height <= 0 ||
                Weight == null || Weight <= 0))
                yield return new ValidationResult("Se as dimensões não forem editáveis, elas devem ser todas preenchidas e devem ter valores positivos.");
        }
    }

    public class ProductTemplateImageViewModel
    {
        private const int size = 380;

        public ProductTemplateImageViewModel()
        {
        }

        public ProductTemplateImageViewModel(string imageUrl)
        {
            Url = new Uri(imageUrl);
        }

        public ProductTemplateImageViewModel(ProductTemplateImage image)
        {
            Url = new Uri(image.Url);
        }

        public Uri Url { get; set; }
        public Uri DisplayUrl => new Uri(ImgHelper.Get(Url.ToString(), size, size));
    }

    public class ProductTemplateIndexViewModel
    {
        public ReadOnlyCollection<ProductTemplateAdminListItemDto> Items { get; set; }
    }

    public static class ProductTemplateViewModelExtensions
    {
        public static string DisplayDimensions(this ProductTemplateAdminListItemDto dto)
        {
            string format(decimal? d) => (d ?? 0m).ToString("N2");
            return $"Comp {format(dto.DimensionsWidth)}cm x Larg {format(dto.DimensionsLength)}cm x Alt {format(dto.DimensionsHeight)}cm | Peso {format(dto.DimensionsWeight)}kg";
        }

        public static decimal GetArea(this ProductTemplateAdminListItemDto dto)
        {
            return dto.DimensionsWidth.GetValueOrDefault() * dto.DimensionsLength.GetValueOrDefault() * dto.DimensionsHeight.GetValueOrDefault();
        }
    }
}