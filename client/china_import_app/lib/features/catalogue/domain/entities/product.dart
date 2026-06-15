import 'product_variant.dart';

/// Domain representation of a catalogue product.
class Product {
  const Product({
    required this.id,
    required this.name,
    required this.categoryId,
    required this.indicativePriceZar,
    this.description,
    this.imageUrls = const <String>[],
    this.variants = const <ProductVariant>[],
  });

  final String id;
  final String name;
  final String? description;
  final String categoryId;
  final double indicativePriceZar;
  final List<String> imageUrls;
  final List<ProductVariant> variants;

  /// First available image, or `null` when the product has none.
  String? get primaryImage => imageUrls.isEmpty ? null : imageUrls.first;

  Product copyWith({
    String? id,
    String? name,
    String? description,
    String? categoryId,
    double? indicativePriceZar,
    List<String>? imageUrls,
    List<ProductVariant>? variants,
  }) {
    return Product(
      id: id ?? this.id,
      name: name ?? this.name,
      description: description ?? this.description,
      categoryId: categoryId ?? this.categoryId,
      indicativePriceZar: indicativePriceZar ?? this.indicativePriceZar,
      imageUrls: imageUrls ?? this.imageUrls,
      variants: variants ?? this.variants,
    );
  }
}
