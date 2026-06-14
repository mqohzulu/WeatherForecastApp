/// Application flavors / environments.
///
/// Selected at build time via `--dart-define=ENV=dev|staging|prod`.
enum Flavor {
  dev,
  staging,
  prod;

  static Flavor fromName(String name) {
    switch (name.toLowerCase()) {
      case 'prod':
      case 'production':
        return Flavor.prod;
      case 'staging':
      case 'stage':
        return Flavor.staging;
      case 'dev':
      case 'development':
      default:
        return Flavor.dev;
    }
  }

  bool get isDev => this == Flavor.dev;
  bool get isStaging => this == Flavor.staging;
  bool get isProd => this == Flavor.prod;

  String get label {
    switch (this) {
      case Flavor.dev:
        return 'Development';
      case Flavor.staging:
        return 'Staging';
      case Flavor.prod:
        return 'Production';
    }
  }
}
