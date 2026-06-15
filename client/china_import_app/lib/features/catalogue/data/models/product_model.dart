import 'package:json_annotation/json_annotation.dart';

import '../../domain/entities/product.dart';
import '../../domain/entities/product_variant.dart';

part 'product_model.g.dart';

/// Data model for a product variant, matching the .NET `ProductVariantDto`.
@JsonSerializable()
class ProductVariantModel {
  const ProductVariantModel({
    required this.id,
    this.size,
    this.colour,
    this.model,
    this.priceZar,
  });

  final String id;
  final String? size;
  final String? colour;
  final String? model;
  final double? priceZar;

  factory ProductVariantModel.fromJson(Map<String, dynamic> json) =>
      _$ProductVariantModelFromJson(json);

  Map<String, dynamic> toJson() => _$ProductVariantModelToJson(this);

  ProductVariant toEntity() => ProductVariant(
        id: id,
        size: size,
        colour: colour,
        model: model,
        priceZar: priceZar,
      );
}

/// Data model for a product, matching the .NET `ProductDto` (camelCase).
@JsonSerializable()
class ProductModel {
  const ProductModel({
    required this.id,
    required this.name,
    required this.categoryId,
    required this.indicativePriceZar,
    this.description,
    this.imageUrls = const <String>[],
    this.variants = const <ProductVariantModel>[],
  });

  final String id;
  final String name;
  final String? description;
  final String categoryId;
  final double indicativePriceZar;
  final List<String> imageUrls;
  final List<ProductVariantModel> variants;

  factory ProductModel.fromJson(Map<String, dynamic> json) =>
      _$ProductModelFromJson(json);

  Map<String, dynamic> toJson() => _$ProductModelToJson(this);

  Product toEntity() => Product(
        id: id,
        name: name,
        description: description,
        categoryId: categoryId,
        indicativePriceZar: indicativePriceZar,
        imageUrls: imageUrls,
        variants: variants.map((v) => v.toEntity()).toList(growable: false),
      );
}
