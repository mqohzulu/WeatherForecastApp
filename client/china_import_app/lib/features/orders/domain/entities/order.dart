import 'order_item.dart';
import 'order_status.dart';
import 'order_status_history.dart';

/// Domain representation of a customer order.
class Order {
  const Order({
    required this.id,
    required this.reference,
    required this.status,
    required this.items,
    required this.totalZar,
    required this.createdAt,
    required this.history,
    required this.isCustomOrder,
    this.notes,
  });

  final String id;
  final String reference;
  final OrderStatus status;
  final List<OrderItem> items;
  final double totalZar;
  final DateTime createdAt;
  final List<OrderStatusHistory> history;
  final String? notes;
  final bool isCustomOrder;

  /// Total number of units across all line items.
  int get itemCount =>
      items.fold<int>(0, (sum, item) => sum + item.quantity);
}
