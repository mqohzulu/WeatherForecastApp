import 'package:json_annotation/json_annotation.dart';

import '../../domain/entities/demand_line.dart';

part 'demand_line_model.g.dart';

/// Wire model for `GET /consolidated-demand` items.
@JsonSerializable()
class DemandLineModel {
  const DemandLineModel({
    required this.productId,
    required this.productName,
    required this.totalQuantity,
    required this.orderCount,
  });

  final String productId;
  final String productName;
  final int totalQuantity;
  final int orderCount;

  factory DemandLineModel.fromJson(Map<String, dynamic> json) =>
      _$DemandLineModelFromJson(json);

  Map<String, dynamic> toJson() => _$DemandLineModelToJson(this);

  DemandLine toEntity() => DemandLine(
        productId: productId,
        productName: productName,
        totalQuantity: totalQuantity,
        orderCount: orderCount,
      );
}
