/// Reusable form validators.
///
/// Validator functions return `null` when valid, or an error message string
/// when invalid — matching Flutter's `TextFormField.validator` contract.
class Validators {
  Validators._();

  /// South African mobile numbers.
  ///
  /// Accepts local format `0XXXXXXXXX` (10 digits starting with 0) or the
  /// E.164 international format `+27XXXXXXXXX`. Spaces are ignored.
  static String? saMobile(String? value) {
    if (value == null || value.trim().isEmpty) {
      return 'Mobile number is required';
    }
    final cleaned = value.replaceAll(RegExp(r'\s+'), '');

    final local = RegExp(r'^0[6-8][0-9]{8}$'); // 06x / 07x / 08x
    final intl = RegExp(r'^\+27[6-8][0-9]{8}$');

    if (local.hasMatch(cleaned) || intl.hasMatch(cleaned)) {
      return null;
    }
    return 'Enter a valid South African mobile number';
  }

  /// Normalises an SA mobile number to E.164 (`+27...`).
  static String normaliseSaMobile(String value) {
    final cleaned = value.replaceAll(RegExp(r'\s+'), '');
    if (cleaned.startsWith('+27')) return cleaned;
    if (cleaned.startsWith('0')) return '+27${cleaned.substring(1)}';
    if (cleaned.startsWith('27')) return '+$cleaned';
    return cleaned;
  }

  static String? otp(String? value, {int length = 6}) {
    if (value == null || value.trim().isEmpty) {
      return 'Enter the verification code';
    }
    if (!RegExp('^[0-9]{$length}\$').hasMatch(value.trim())) {
      return 'Enter the $length-digit code';
    }
    return null;
  }

  static String? required(String? value, {String field = 'This field'}) {
    if (value == null || value.trim().isEmpty) {
      return '$field is required';
    }
    return null;
  }

  static String? minLength(String? value, int min, {String field = 'This field'}) {
    if (value == null || value.trim().length < min) {
      return '$field must be at least $min characters';
    }
    return null;
  }

  static String? positiveInt(String? value, {String field = 'Quantity'}) {
    final parsed = int.tryParse(value?.trim() ?? '');
    if (parsed == null || parsed <= 0) {
      return '$field must be a positive number';
    }
    return null;
  }
}
