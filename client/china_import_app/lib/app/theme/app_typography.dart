import 'package:flutter/material.dart';

import 'app_colors.dart';

/// Centralised text styles built on Material 3's [TextTheme].
class AppTypography {
  AppTypography._();

  static TextTheme textTheme(TextTheme base) {
    return base
        .copyWith(
          displaySmall: base.displaySmall?.copyWith(fontWeight: FontWeight.w700),
          headlineMedium:
              base.headlineMedium?.copyWith(fontWeight: FontWeight.w700),
          headlineSmall: base.headlineSmall?.copyWith(fontWeight: FontWeight.w600),
          titleLarge: base.titleLarge?.copyWith(fontWeight: FontWeight.w600),
          titleMedium: base.titleMedium?.copyWith(fontWeight: FontWeight.w600),
          labelLarge: base.labelLarge?.copyWith(fontWeight: FontWeight.w600),
        )
        .apply(
          bodyColor: AppColors.textPrimary,
          displayColor: AppColors.textPrimary,
        );
  }
}
