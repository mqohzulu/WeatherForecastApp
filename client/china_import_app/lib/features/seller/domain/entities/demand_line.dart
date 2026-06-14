/// Aggregated demand for a single product across orders.
class DemandLine {
  const DemandLine({
    required this.productId,
    required this.productName,
    required this.totalQuantity,
    required this.orderCount,
  });

  final String productId;
  final String productName;
  final int totalQuantity;
  final int orderCount;
}
