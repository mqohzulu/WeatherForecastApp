import 'package:json_annotation/json_annotation.dart';

import '../../domain/entities/user.dart';

part 'auth_response_model.g.dart';

/// Response from `POST /auth/request-otp`.
@JsonSerializable()
class RequestOtpResponse {
  const RequestOtpResponse({
    required this.verificationId,
    this.expiresInSeconds,
  });

  final String verificationId;
  final int? expiresInSeconds;

  factory RequestOtpResponse.fromJson(Map<String, dynamic> json) =>
      _$RequestOtpResponseFromJson(json);

  Map<String, dynamic> toJson() => _$RequestOtpResponseToJson(this);
}

/// User payload nested inside the verify-otp / me responses.
@JsonSerializable()
class UserModel {
  const UserModel({
    required this.id,
    required this.phoneNumber,
    required this.role,
    this.displayName,
    this.email,
  });

  final String id;
  final String phoneNumber;
  final String role;
  final String? displayName;
  final String? email;

  factory UserModel.fromJson(Map<String, dynamic> json) =>
      _$UserModelFromJson(json);

  Map<String, dynamic> toJson() => _$UserModelToJson(this);

  User toEntity() => User(
        id: id,
        phoneNumber: phoneNumber,
        role: UserRole.fromString(role),
        displayName: displayName,
        email: email,
      );
}

/// Response from `POST /auth/verify-otp` and `POST /auth/refresh`.
@JsonSerializable()
class AuthResponseModel {
  const AuthResponseModel({
    required this.accessToken,
    required this.refreshToken,
    required this.user,
  });

  final String accessToken;
  final String refreshToken;
  final UserModel user;

  factory AuthResponseModel.fromJson(Map<String, dynamic> json) =>
      _$AuthResponseModelFromJson(json);

  Map<String, dynamic> toJson() => _$AuthResponseModelToJson(this);
}
