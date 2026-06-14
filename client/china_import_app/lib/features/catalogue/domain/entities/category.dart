/// Domain representation of a product category.
class Category {
  const Category({
    required this.id,
    required this.name,
    this.imageUrl,
    this.productCount,
  });

  final String id;
  final String name;
  final String? imageUrl;
  final int? productCount;

  Category copyWith({
    String? id,
    String? name,
    String? imageUrl,
    int? productCount,
  }) {
    return Category(
      id: id ?? this.id,
      name: name ?? this.name,
      imageUrl: imageUrl ?? this.imageUrl,
      productCount: productCount ?? this.productCount,
    );
  }
}
