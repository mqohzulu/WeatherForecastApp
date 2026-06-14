import 'dart:developer' as developer;

import '../config/app_config.dart';

/// Lightweight logging facade.
///
/// In production builds (`enableLogging == false`) only warnings and
/// errors are emitted. Uses `dart:developer` rather than `print` so that
/// the `avoid_print` lint stays satisfied.
class AppLogger {
  const AppLogger(this._tag);

  final String _tag;

  bool get _enabled {
    try {
      return AppConfig.instance.enableLogging;
    } catch (_) {
      // Config not initialised yet (e.g. very early bootstrap) — log anyway.
      return true;
    }
  }

  void d(String message) {
    if (_enabled) _log(message, level: 500);
  }

  void i(String message) {
    if (_enabled) _log(message, level: 800);
  }

  void w(String message) => _log(message, level: 900);

  void e(String message, [Object? error, StackTrace? stackTrace]) {
    _log(message, level: 1000, error: error, stackTrace: stackTrace);
  }

  void _log(
    String message, {
    required int level,
    Object? error,
    StackTrace? stackTrace,
  }) {
    developer.log(
      message,
      name: _tag,
      level: level,
      error: error,
      stackTrace: stackTrace,
    );
  }
}
