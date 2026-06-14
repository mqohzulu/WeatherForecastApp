import 'package:json_annotation/json_annotation.dart';

import '../../domain/entities/payment_request.dart';

part 'payment_request_model.g.dart';

/// Wire model for `GET /payment-requests` items.
@JsonSerializable()
class PaymentRequestModel {
  const PaymentRequestModel({
    required this.id,
    required this.orderId,
    required this.amountZar,
    required this.status,
    required this.createdAt,
    this.reference,
  });

  final String id;
  final String orderId;
  final double amountZar;
  final String status;
  final DateTime createdAt;
  final String? reference;

  factory PaymentRequestModel.fromJson(Map<String, dynamic> json) =>
      _$PaymentRequestModelFromJson(json);

  Map<String, dynamic> toJson() => _$PaymentRequestModelToJson(this);

  PaymentRequest toEntity() => PaymentRequest(
        id: id,
        orderId: orderId,
        amountZar: amountZar,
        status: status,
        createdAt: createdAt,
        reference: reference,
      );
}
