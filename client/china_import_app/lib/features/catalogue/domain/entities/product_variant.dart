/// A purchasable variant of a [Product] (size / colour / model combination).
class ProductVariant {
  const ProductVariant({
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

  /// Human-friendly label joining the non-null attributes, e.g. `XL · Blue`.
  String get label {
    final parts = <String>[
      if (model != null && model!.isNotEmpty) model!,
      if (size != null && size!.isNotEmpty) size!,
      if (colour != null && colour!.isNotEmpty) colour!,
    ];
    return parts.join(' · ');
  }

  ProductVariant copyWith({
    String? id,
    String? size,
    String? colour,
    String? model,
    double? priceZar,
  }) {
    return ProductVariant(
      id: id ?? this.id,
      size: size ?? this.size,
      colour: colour ?? this.colour,
      model: model ?? this.model,
      priceZar: priceZar ?? this.priceZar,
    );
  }
}
