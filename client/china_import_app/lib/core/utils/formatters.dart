import 'package:intl/intl.dart';

import '../constants/app_constants.dart';

/// Centralised display formatting (currency, dates, etc.).
class Formatters {
  Formatters._();

  static final NumberFormat _zar = NumberFormat.currency(
    locale: AppConstants.defaultLocale,
    symbol: '${AppConstants.currencySymbol} ',
    decimalDigits: 2,
  );

  static final NumberFormat _zarCompact = NumberFormat.compactCurrency(
    locale: AppConstants.defaultLocale,
    symbol: '${AppConstants.currencySymbol} ',
  );

  static final DateFormat _date = DateFormat('d MMM yyyy', AppConstants.defaultLocale);
  static final DateFormat _dateTime =
      DateFormat('d MMM yyyy, HH:mm', AppConstants.defaultLocale);
  static final DateFormat _time = DateFormat('HH:mm', AppConstants.defaultLocale);

  /// Formats a ZAR amount, e.g. `R 1 299.00`.
  static String zar(num amount) => _zar.format(amount);

  /// Compact ZAR, e.g. `R 1,3K`.
  static String zarCompact(num amount) => _zarCompact.format(amount);

  static String date(DateTime value) => _date.format(value.toLocal());

  static String dateTime(DateTime value) => _dateTime.format(value.toLocal());

  static String time(DateTime value) => _time.format(value.toLocal());

  /// Human-friendly relative time, e.g. "just now", "5m ago", "2d ago".
  static String relative(DateTime value) {
    final diff = DateTime.now().difference(value.toLocal());
    if (diff.inSeconds < 60) return 'just now';
    if (diff.inMinutes < 60) return '${diff.inMinutes}m ago';
    if (diff.inHours < 24) return '${diff.inHours}h ago';
    if (diff.inDays < 7) return '${diff.inDays}d ago';
    return date(value);
  }
}
