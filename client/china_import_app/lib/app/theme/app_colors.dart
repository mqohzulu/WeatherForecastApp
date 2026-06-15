import 'package:flutter/material.dart';

/// Brand colour palette.
///
/// Primary draws on a deep imperial red (a nod to the China trade route)
/// balanced with a warm gold accent and neutral surfaces.
class AppColors {
  AppColors._();

  // Brand
  static const Color primary = Color(0xFFC0392B); // imperial red
  static const Color primaryDark = Color(0xFF8E2A20);
  static const Color secondary = Color(0xFFD4A017); // gold accent
  static const Color tertiary = Color(0xFF1F6F5C); // jade green

  // Neutrals
  static const Color background = Color(0xFFF7F6F4);
  static const Color surface = Color(0xFFFFFFFF);
  static const Color surfaceVariant = Color(0xFFEFEDEA);
  static const Color outline = Color(0xFFCBC7C2);

  // Text
  static const Color textPrimary = Color(0xFF1C1B1A);
  static const Color textSecondary = Color(0xFF5F5C58);

  // Semantic
  static const Color success = Color(0xFF2E7D32);
  static const Color warning = Color(0xFFED8B00);
  static const Color error = Color(0xFFC62828);
  static const Color info = Color(0xFF1565C0);

  // Order status colours (keyed by OrderStatus.name in the orders feature).
  static const Color statusPlaced = Color(0xFF607D8B);
  static const Color statusConfirmed = Color(0xFF1565C0);
  static const Color statusBeingSourced = Color(0xFF6A1B9A);
  static const Color statusShipped = Color(0xFF00838F);
  static const Color statusArrivedInSa = Color(0xFF2E7D32);
  static const Color statusReadyForCollection = Color(0xFFD4A017);
  static const Color statusCompleted = Color(0xFF1B5E20);
  static const Color statusCancelled = Color(0xFF9E9E9E);
  static const Color statusRejected = Color(0xFFC62828);
}
