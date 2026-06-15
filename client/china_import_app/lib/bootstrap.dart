import 'dart:async';

import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import 'app/app.dart';
import 'core/config/app_config.dart';
import 'core/utils/logger.dart';

/// Initialises configuration + global error handling, then runs the app
/// inside a Riverpod [ProviderScope].
Future<void> bootstrap() async {
  const log = AppLogger('bootstrap');

  await runZonedGuarded(
    () async {
      WidgetsFlutterBinding.ensureInitialized();

      // Resolve the active environment from --dart-define=ENV=...
      final config = AppConfig.init();
      log.i('Starting ${config.flavor.label} build -> ${config.apiUrl}');

      // Forward framework errors to our logger.
      FlutterError.onError = (FlutterErrorDetails details) {
        FlutterError.presentError(details);
        log.e('FlutterError', details.exception, details.stack);
      };

      PlatformDispatcher.instance.onError = (error, stack) {
        log.e('PlatformDispatcher error', error, stack);
        return true;
      };

      runApp(
        const ProviderScope(
          child: ChinaImportApp(),
        ),
      );
    },
    (error, stack) {
      log.e('Uncaught zone error', error, stack);
      if (kDebugMode) {
        // Surface in debug; swallowed (logged) in release.
        debugPrintStack(stackTrace: stack, label: error.toString());
      }
    },
  );
}
