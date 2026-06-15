/// A single line item within an order.
class OrderItem {
  const OrderItem({
    required this.id,
    required this.productId,
    required this.productName,
    required this.quantity,
    required this.unitPriceZar,
    this.variantLabel,
  });

  final String id;
  final String productId;
  final String productName;
  final String? variantLabel;
  final int quantity;
  final double unitPriceZar;

  /// Total price for this line (unit price x quantity).
  double get lineTotalZar => unitPriceZar * quantity;
}
