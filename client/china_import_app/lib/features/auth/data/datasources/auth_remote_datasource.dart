import 'package:dio/dio.dart';

import '../../../../core/constants/api_endpoints.dart';
import '../models/auth_response_model.dart';

/// Remote calls for the auth feature.
abstract interface class AuthRemoteDataSource {
  Future<RequestOtpResponse> requestOtp(String phoneNumber);

  Future<AuthResponseModel> verifyOtp({
    required String phoneNumber,
    required String verificationId,
    required String code,
  });

  Future<UserModel> me();

  Future<void> logout();
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  AuthRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<RequestOtpResponse> requestOtp(String phoneNumber) async {
    final res = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.requestOtp,
      data: {'phoneNumber': phoneNumber},
    );
    return RequestOtpResponse.fromJson(res.data!);
  }

  @override
  Future<AuthResponseModel> verifyOtp({
    required String phoneNumber,
    required String verificationId,
    required String code,
  }) async {
    final res = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.verifyOtp,
      data: {
        'phoneNumber': phoneNumber,
        'verificationId': verificationId,
        'code': code,
      },
    );
    return AuthResponseModel.fromJson(res.data!);
  }

  @override
  Future<UserModel> me() async {
    final res = await _dio.get<Map<String, dynamic>>(ApiEndpoints.me);
    return UserModel.fromJson(res.data!);
  }

  @override
  Future<void> logout() async {
    await _dio.post<void>(ApiEndpoints.logout);
  }
}
