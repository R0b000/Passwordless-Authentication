namespace Test
{
    public static class HtmlCode
    {
        public static readonly string Source = """
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8" />
    <title>Invoice</title>
</head>
<body>
    <style>
        * {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            width: 794px;
            margin: 0 auto;
            padding: 30px;
            font-family: Arial, Helvetica, sans-serif;
            color: #1a1a1a;
            background-color: #ffffff;
            font-size: 13px;
            line-height: 1.4;
        }

        .header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            border-bottom: 2px solid #2563eb;
            padding-bottom: 15px;
            margin-bottom: 20px;
        }

        .brand h1 {
            font-size: 22px;
            color: #2563eb;
            font-weight: 700;
            text-transform: uppercase;
        }

        .brand p {
            color: #666;
            font-size: 12px;
            margin-top: 2px;
        }

        .doc-meta {
            text-align: right;
        }

        .doc-meta h2 {
            font-size: 18px;
            color: #1a1a1a;
            margin-bottom: 4px;
        }

        .doc-meta p {
            color: #555;
            font-size: 12px;
        }

        .info-grid {
            display: flex;
            justify-content: space-between;
            margin-bottom: 25px;
        }

        .info-col h3 {
            font-size: 11px;
            text-transform: uppercase;
            color: #888;
            margin-bottom: 4px;
            letter-spacing: 0.5px;
        }

        .info-col p {
            font-size: 12px;
            color: #222;
            line-height: 1.4;
        }

        table {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 20px;
        }

        th {
            background-color: #f3f4f6;
            color: #374151;
            font-weight: 600;
            text-align: left;
            padding: 8px 10px;
            font-size: 11px;
            text-transform: uppercase;
            border-bottom: 1px solid #e5e7eb;
        }

        td {
            padding: 10px;
            border-bottom: 1px solid #e5e7eb;
            color: #374151;
            font-size: 12px;
        }

        .text-right { text-align: right; }
        .text-center { text-align: center; }

        .totals-wrap {
            display: flex;
            justify-content: flex-end;
            margin-bottom: 20px;
        }

        .totals-table {
            width: 240px;
        }

        .totals-table td {
            padding: 4px 0;
            border: none;
        }

        .totals-table .grand-total {
            font-size: 15px;
            font-weight: bold;
            color: #2563eb;
            border-top: 2px solid #e5e7eb;
            padding-top: 8px;
        }

        .notes {
            background-color: #f9fafb;
            border-left: 4px solid #2563eb;
            padding: 10px 14px;
            border-radius: 0 4px 4px 0;
        }

        .notes h4 {
            font-size: 11px;
            text-transform: uppercase;
            color: #374151;
            margin-bottom: 2px;
        }

        .notes p {
            font-size: 11px;
            color: #6b7280;
        }
    </style>

    <!-- Header -->
    <div class="header">
        <div class="brand">
            <h1>NovaPulse Inc.</h1>
            <p>123 Innovation Way, Suite 400</p>
            <p>San Francisco, CA 94105</p>
            <p>support@novapulse.io</p>
        </div>
        <div class="doc-meta">
            <h2>INVOICE</h2>
            <p><strong>Invoice #:</strong> INV-2026-089</p>
            <p><strong>Date:</strong> August 16, 2026</p>
            <p><strong>Due Date:</strong> August 30, 2026</p>
        </div>
    </div>

    <!-- Client & Project Details -->
    <div class="info-grid">
        <div class="info-col">
            <h3>Billed To</h3>
            <p><strong>Acme Corporation</strong></p>
            <p>Attn: John Doe</p>
            <p>456 Market Street, Floor 12</p>
            <p>john@acme.com</p>
        </div>
        <div class="info-col text-right">
            <h3>Payment Details</h3>
            <p><strong>Method:</strong> Bank Transfer (ACH)</p>
            <p><strong>Account:</strong> **** **** 4892</p>
            <p><strong>Status:</strong> <span style="color: #16a34a; font-weight: bold;">Unpaid</span></p>
        </div>
    </div>

    <!-- Line Items Table -->
    <table>
        <thead>
            <tr>
                <th style="width: 50%;">Description</th>
                <th class="text-center" style="width: 15%;">Hours / Qty</th>
                <th class="text-right" style="width: 15%;">Rate</th>
                <th class="text-right" style="width: 20%;">Amount</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>
                    <strong>Performance Marketing Sprint</strong><br />
                    <span style="font-size: 11px; color: #6b7280;">Full-funnel campaign optimization and creative asset rotation.</span>
                </td>
                <td class="text-center">1</td>
                <td class="text-right">$2,500.00</td>
                <td class="text-right">$2,500.00</td>
            </tr>
            <tr>
                <td>
                    <strong>SEO & Technical Architecture Audit</strong><br />
                    <span style="font-size: 11px; color: #6b7280;">Core Web Vitals remediation and metadata restructuring.</span>
                </td>
                <td class="text-center">20 hrs</td>
                <td class="text-right">$100.00</td>
                <td class="text-right">$2,000.00</td>
            </tr>
            <tr>
                <td>
                    <strong>Custom Landing Page Template Build</strong><br />
                    <span style="font-size: 11px; color: #6b7280;">Responsive design and integration.</span>
                </td>
                <td class="text-center">1</td>
                <td class="text-right">$1,200.00</td>
                <td class="text-right">$1,200.00</td>
            </tr>
        </tbody>
    </table>

    <!-- Totals -->
    <div class="totals-wrap">
        <table class="totals-table">
            <tr>
                <td class="text-right" style="color: #6b7280;">Subtotal:</td>
                <td class="text-right" style="font-weight: 600;">$5,700.00</td>
            </tr>
            <tr>
                <td class="text-right" style="color: #6b7280;">Tax (0%):</td>
                <td class="text-right" style="font-weight: 600;">$0.00</td>
            </tr>
            <tr>
                <td class="text-right grand-total">Total Due:</td>
                <td class="text-right grand-total">$5,700.00</td>
            </tr>
        </table>
    </div>

    <!-- Notes & Terms -->
    <div class="notes">
        <h4>Terms & Instructions</h4>
        <p>Please send payments within 14 days of invoice receipt. Make checks or direct bank transfers payable to <strong>NovaPulse Inc.</strong></p>
    </div>
</body>
</html>
""";
    }
}