import 'flavor_config.dart';

/// Environment-based application configuration.
///
/// The active environment is chosen at build/run time:
/// `flutter run --dart-define=ENV=dev`
/// `flutter build apk --dart-define=ENV=prod`
class AppConfig {
  AppConfig._({
    required this.flavor,
    required this.apiBaseUrl,
    required this.connectTimeout,
    required this.receiveTimeout,
    required this.enableLogging,
  });

  final Flavor flavor;
  final String apiBaseUrl;
  final Duration connectTimeout;
  final Duration receiveTimeout;
  final bool enableLogging;

  /// API version prefix appended after [apiBaseUrl].
  static const String apiVersion = '/api/v1';

  /// Fully-qualified base URL including the version prefix.
  String get apiUrl => '$apiBaseUrl$apiVersion';

  static AppConfig? _instance;
  static AppConfig get instance {
    final config = _instance;
    if (config == null) {
      throw StateError(
        'AppConfig has not been initialised. Call AppConfig.init() first.',
      );
    }
    return config;
  }

  /// Initialises the singleton from the `ENV` dart-define.
  static AppConfig init() {
    const envName = String.fromEnvironment('ENV', defaultValue: 'dev');
    final flavor = Flavor.fromName(envName);
    _instance = _build(flavor);
    return _instance!;
  }

  static AppConfig _build(Flavor flavor) {
    switch (flavor) {
      case Flavor.dev:
        return AppConfig._(
          flavor: flavor,
          // 10.0.2.2 is the Android emulator alias for the host machine.
          apiBaseUrl: 'http://10.0.2.2:5000',
          connectTimeout: const Duration(seconds: 20),
          receiveTimeout: const Duration(seconds: 30),
          enableLogging: true,
        );
      case Flavor.staging:
        return AppConfig._(
          flavor: flavor,
          apiBaseUrl: 'https://staging-api.china-import.example.co.za',
          connectTimeout: const Duration(seconds: 20),
          receiveTimeout: const Duration(seconds: 30),
          enableLogging: true,
        );
      case Flavor.prod:
        return AppConfig._(
          flavor: flavor,
          apiBaseUrl: 'https://api.china-import.example.co.za',
          connectTimeout: const Duration(seconds: 15),
          receiveTimeout: const Duration(seconds: 25),
          enableLogging: false,
        );
    }
  }
}
