import 'package:json_annotation/json_annotation.dart';

import '../../domain/entities/order.dart';
import '../../domain/entities/order_item.dart';
import '../../domain/entities/order_status.dart';
import '../../domain/entities/order_status_history.dart';

part 'order_model.g.dart';

/// A single line item as returned by the API.
@JsonSerializable()
class OrderItemModel {
  const OrderItemModel({
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

  factory OrderItemModel.fromJson(Map<String, dynamic> json) =>
      _$OrderItemModelFromJson(json);

  Map<String, dynamic> toJson() => _$OrderItemModelToJson(this);

  OrderItem toEntity() => OrderItem(
        id: id,
        productId: productId,
        productName: productName,
        variantLabel: variantLabel,
        quantity: quantity,
        unitPriceZar: unitPriceZar,
      );
}

/// A status transition entry as returned by the API.
@JsonSerializable()
class OrderStatusHistoryModel {
  const OrderStatusHistoryModel({
    required this.status,
    required this.timestamp,
    this.note,
  });

  final String status;
  final String timestamp;
  final String? note;

  factory OrderStatusHistoryModel.fromJson(Map<String, dynamic> json) =>
      _$OrderStatusHistoryModelFromJson(json);

  Map<String, dynamic> toJson() => _$OrderStatusHistoryModelToJson(this);

  OrderStatusHistory toEntity() => OrderStatusHistory(
        status: OrderStatus.fromJson(status),
        timestamp: DateTime.parse(timestamp),
        note: note,
      );
}

/// An order as returned by the API.
@JsonSerializable()
class OrderModel {
  const OrderModel({
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
  final String status;
  final List<OrderItemModel> items;
  final double totalZar;
  final String createdAt;
  final List<OrderStatusHistoryModel> history;
  final String? notes;
  final bool isCustomOrder;

  factory OrderModel.fromJson(Map<String, dynamic> json) =>
      _$OrderModelFromJson(json);

  Map<String, dynamic> toJson() => _$OrderModelToJson(this);

  Order toEntity() => Order(
        id: id,
        reference: reference,
        status: OrderStatus.fromJson(status),
        items: items.map((e) => e.toEntity()).toList(growable: false),
        totalZar: totalZar,
        createdAt: DateTime.parse(createdAt),
        history: history.map((e) => e.toEntity()).toList(growable: false),
        notes: notes,
        isCustomOrder: isCustomOrder,
      );
}
