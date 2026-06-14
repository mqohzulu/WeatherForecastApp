/// A single line in the local (client-side) cart.
///
/// Lines are merged by [lineKey], so the same product with a different
/// variant counts as a distinct line.
class CartItem {
  const CartItem({
    required this.productId,
    required this.productName,
    this.imageUrl,
    this.variantId,
    this.variantLabel,
    required this.unitPriceZar,
    required this.quantity,
  });

  final String productId;
  final String productName;
  final String? imageUrl;
  final String? variantId;
  final String? variantLabel;
  final double unitPriceZar;
  final int quantity;

  /// Stable identity used to merge equivalent lines.
  String get lineKey => '$productId:${variantId ?? ''}';

  /// Total price for this line, in ZAR.
  double get lineTotalZar => unitPriceZar * quantity;

  CartItem copyWith({int? quantity}) {
    return CartItem(
      productId: productId,
      productName: productName,
      imageUrl: imageUrl,
      variantId: variantId,
      variantLabel: variantLabel,
      unitPriceZar: unitPriceZar,
      quantity: quantity ?? this.quantity,
    );
  }
}
