/// The two roles supported by the platform.
enum UserRole {
  customer,
  seller;

  static UserRole fromString(String? value) {
    switch (value?.toLowerCase()) {
      case 'seller':
      case 'admin':
        return UserRole.seller;
      case 'customer':
      default:
        return UserRole.customer;
    }
  }

  bool get isSeller => this == UserRole.seller;
  bool get isCustomer => this == UserRole.customer;
}

/// Domain representation of an authenticated user.
class User {
  const User({
    required this.id,
    required this.phoneNumber,
    required this.role,
    this.displayName,
    this.email,
  });

  final String id;
  final String phoneNumber;
  final UserRole role;
  final String? displayName;
  final String? email;

  User copyWith({
    String? id,
    String? phoneNumber,
    UserRole? role,
    String? displayName,
    String? email,
  }) {
    return User(
      id: id ?? this.id,
      phoneNumber: phoneNumber ?? this.phoneNumber,
      role: role ?? this.role,
      displayName: displayName ?? this.displayName,
      email: email ?? this.email,
    );
  }
}
