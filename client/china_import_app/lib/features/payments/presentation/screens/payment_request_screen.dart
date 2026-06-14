import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:image_picker/image_picker.dart';

import '../../../../core/utils/formatters.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/error_view.dart';
import '../../../../core/widgets/loading_indicator.dart';
import '../../domain/entities/payment_request.dart';
import '../providers/payments_provider.dart';

// NOTE (manual phase): proof upload is wired to the API but the banking
// details below are static placeholders pending finance sign-off.
class PaymentRequestScreen extends ConsumerStatefulWidget {
  const PaymentRequestScreen({super.key, this.orderId});

  final String? orderId;

  @override
  ConsumerState<PaymentRequestScreen> createState() =>
      _PaymentRequestScreenState();
}

class _PaymentRequestScreenState extends ConsumerState<PaymentRequestScreen> {
  bool _uploading = false;

  Future<void> _uploadProof(String paymentRequestId) async {
    if (_uploading) return;
    final picker = ImagePicker();
    final file = await picker.pickImage(source: ImageSource.gallery);
    if (file == null || !mounted) return;

    setState(() => _uploading = true);
    final result = await ref
        .read(paymentsRepositoryProvider)
        .uploadProof(paymentRequestId, file.path);

    if (!mounted) return;
    setState(() => _uploading = false);
    result.match(
      (failure) {
        ScaffoldMessenger.of(context)
          ..hideCurrentSnackBar()
          ..showSnackBar(SnackBar(content: Text(failure.message)));
      },
      (_) {
        ScaffoldMessenger.of(context)
          ..hideCurrentSnackBar()
          ..showSnackBar(
            const SnackBar(content: Text('Proof of payment uploaded')),
          );
        ref.invalidate(paymentRequestsProvider(widget.orderId));
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    final asyncRequests =
        ref.watch(paymentRequestsProvider(widget.orderId));

    return Scaffold(
      appBar: AppBar(title: const Text('Payment')),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          const _BankingDetailsCard(),
          const SizedBox(height: 16),
          Text(
            'Payment requests',
            style: Theme.of(context).textTheme.titleMedium,
          ),
          const SizedBox(height: 8),
          asyncRequests.when(
            loading: () => const Padding(
              padding: EdgeInsets.symmetric(vertical: 32),
              child: LoadingIndicator(),
            ),
            error: (error, _) => ErrorView(
              message: error.toString(),
              onRetry: () =>
                  ref.invalidate(paymentRequestsProvider(widget.orderId)),
            ),
            data: (requests) {
              if (requests.isEmpty) {
                return const Padding(
                  padding: EdgeInsets.symmetric(vertical: 24),
                  child: Center(child: Text('No payment requests')),
                );
              }
              return Column(
                children: [
                  for (final request in requests)
                    _PaymentRequestTile(
                      request: request,
                      uploading: _uploading,
                      onUpload: () => _uploadProof(request.id),
                    ),
                ],
              );
            },
          ),
        ],
      ),
    );
  }
}

class _BankingDetailsCard extends StatelessWidget {
  const _BankingDetailsCard();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Icon(Icons.account_balance, color: theme.colorScheme.primary),
                const SizedBox(width: 8),
                Text('Banking details', style: theme.textTheme.titleMedium),
              ],
            ),
            const SizedBox(height: 12),
            const _DetailRow(label: 'Bank', value: 'First National Bank'),
            const _DetailRow(label: 'Account name', value: 'China Import (Pty) Ltd'),
            const _DetailRow(label: 'Account number', value: '6200 0000 000'),
            const _DetailRow(label: 'Branch code', value: '250655'),
            const SizedBox(height: 8),
            Text(
              'Use your order reference as the payment reference.',
              style: theme.textTheme.bodySmall
                  ?.copyWith(color: theme.colorScheme.outline),
            ),
          ],
        ),
      ),
    );
  }
}

class _DetailRow extends StatelessWidget {
  const _DetailRow({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 2),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 120,
            child: Text(
              label,
              style: theme.textTheme.bodyMedium
                  ?.copyWith(color: theme.colorScheme.outline),
            ),
          ),
          Expanded(
            child: Text(value, style: theme.textTheme.bodyMedium),
          ),
        ],
      ),
    );
  }
}

class _PaymentRequestTile extends StatelessWidget {
  const _PaymentRequestTile({
    required this.request,
    required this.uploading,
    required this.onUpload,
  });

  final PaymentRequest request;
  final bool uploading;
  final VoidCallback onUpload;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Card(
      margin: const EdgeInsets.symmetric(vertical: 6),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    Formatters.zar(request.amountZar),
                    style: theme.textTheme.titleMedium,
                  ),
                ),
                Chip(label: Text(request.status)),
              ],
            ),
            const SizedBox(height: 4),
            Text(
              Formatters.dateTime(request.createdAt),
              style: theme.textTheme.bodySmall
                  ?.copyWith(color: theme.colorScheme.outline),
            ),
            if (request.reference != null) ...[
              const SizedBox(height: 2),
              Text(
                'Ref: ${request.reference}',
                style: theme.textTheme.bodySmall,
              ),
            ],
            const SizedBox(height: 12),
            AppButton(
              label: 'Upload proof of payment',
              icon: Icons.upload_file,
              isLoading: uploading,
              onPressed: onUpload,
            ),
          ],
        ),
      ),
    );
  }
}
