using MechanicShop.Application.Common.Interfaces;
using MechanicShop.Domain.WorkOrders.Billing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MechanicShop.Infrastructure.Services;

public sealed class InvoicePdfGenerator : IInvoicePdfGenerator
{
    //  invoice palette
    private const string Primary = "#172033";
    private const string Accent = "#F59E0B";
    private const string AccentDark = "#D97706";
    private const string TextDark = "#1F2937";
    private const string TextMedium = "#6B7280";
    private const string Border = "#E5E7EB";
    private const string Surface = "#F8FAFC";
    private const string White = "#FFFFFF";

    public byte[] Generate(Invoice invoice)
    {
        return Document
            .Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.MarginHorizontal(42);
                    page.MarginVertical(36);

                    page.DefaultTextStyle(style => style.FontFamily("Arial").FontColor(TextDark));

                    page.Header().Element(BuildHeader(invoice));
                    page.Content().Element(BuildInvoiceContent(invoice));
                    page.Footer().Element(BuildFooter());
                });
            })
            .GeneratePdf();
    }

    private Action<IContainer> BuildHeader(Invoice invoice) =>
        header =>
        {
            header.Column(column =>
            {
                // Main header
                column
                    .Item()
                    .Background(Primary)
                    .Padding(24)
                    .Row(row =>
                    {
                        // Brand
                        row.RelativeItem()
                            .Column(brand =>
                            {
                                brand
                                    .Item()
                                    .Text("MECHANIC SHOP")
                                    .FontSize(24)
                                    .Bold()
                                    .FontColor(White);

                                brand
                                    .Item()
                                    .PaddingTop(4)
                                    .Text("AUTOMOTIVE SERVICE & REPAIR")
                                    .FontSize(9)
                                    .LetterSpacing(1.2f)
                                    .FontColor("#CBD5E1");
                            });

                        // Invoice information
                        row.ConstantItem(190)
                            .AlignRight()
                            .Column(details =>
                            {
                                details
                                    .Item()
                                    .AlignRight()
                                    .Text("INVOICE")
                                    .FontSize(11)
                                    .Bold()
                                    .FontColor(Accent);

                                details
                                    .Item()
                                    .PaddingTop(2)
                                    .AlignRight()
                                    .Text($"#{invoice.Id.ToString()[..8].ToUpperInvariant()}")
                                    .FontSize(22)
                                    .Bold()
                                    .FontColor(White);

                                details
                                    .Item()
                                    .PaddingTop(6)
                                    .AlignRight()
                                    .Text($"Issued {invoice.IssuedAtUtc:MMM dd, yyyy}")
                                    .FontSize(9)
                                    .FontColor("#CBD5E1");
                            });
                    });

                // Status strip
                column
                    .Item()
                    .Background(Surface)
                    .BorderBottom(1)
                    .BorderColor(Border)
                    .PaddingHorizontal(24)
                    .PaddingVertical(10)
                    .Row(row =>
                    {
                        row.RelativeItem()
                            .Text("PAYMENT STATUS")
                            .FontSize(8)
                            .Bold()
                            .LetterSpacing(1)
                            .FontColor(TextMedium);

                        row.AutoItem()
                            .Background(GetStatusBackgroundColor(invoice.Status.ToString()))
                            .PaddingHorizontal(12)
                            .PaddingVertical(5)
                            .Text(invoice.Status.ToString().ToUpperInvariant())
                            .FontSize(8)
                            .Bold()
                            .FontColor(GetStatusColor(invoice.Status.ToString()));
                    });

                column.Item().Height(28);
            });
        };

    private Action<IContainer> BuildInvoiceContent(Invoice invoice) =>
        content =>
        {
            content.Column(column =>
            {
                // Section title
                column
                    .Item()
                    .PaddingBottom(12)
                    .Text("SERVICE DETAILS")
                    .FontSize(11)
                    .Bold()
                    .LetterSpacing(1)
                    .FontColor(Primary);

                // Invoice items table
                column
                    .Item()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(4.5f);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        // Header
                        table.Header(header =>
                        {
                            header
                                .Cell()
                                .Background(Primary)
                                .PaddingHorizontal(12)
                                .PaddingVertical(10)
                                .Text("DESCRIPTION")
                                .FontSize(9)
                                .Bold()
                                .FontColor(White);

                            header
                                .Cell()
                                .Background(Primary)
                                .PaddingHorizontal(8)
                                .PaddingVertical(10)
                                .AlignCenter()
                                .Text("QTY")
                                .FontSize(9)
                                .Bold()
                                .FontColor(White);

                            header
                                .Cell()
                                .Background(Primary)
                                .PaddingHorizontal(8)
                                .PaddingVertical(10)
                                .AlignRight()
                                .Text("UNIT PRICE")
                                .FontSize(9)
                                .Bold()
                                .FontColor(White);

                            header
                                .Cell()
                                .Background(Primary)
                                .PaddingHorizontal(12)
                                .PaddingVertical(10)
                                .AlignRight()
                                .Text("AMOUNT")
                                .FontSize(9)
                                .Bold()
                                .FontColor(White);
                        });

                        var isEvenRow = false;

                        foreach (var item in invoice.InvoiceLineItems)
                        {
                            var backgroundColor = isEvenRow ? Surface : White;

                            // Description
                            table
                                .Cell()
                                .Background(backgroundColor)
                                .BorderBottom(1)
                                .BorderColor(Border)
                                .PaddingHorizontal(12)
                                .PaddingVertical(11)
                                .Text(item.Description)
                                .FontSize(10)
                                .FontColor(TextDark);

                            // Quantity
                            table
                                .Cell()
                                .Background(backgroundColor)
                                .BorderBottom(1)
                                .BorderColor(Border)
                                .PaddingHorizontal(8)
                                .PaddingVertical(11)
                                .AlignCenter()
                                .Text(item.Quantity.ToString())
                                .FontSize(10)
                                .FontColor(TextDark);

                            // Unit price
                            table
                                .Cell()
                                .Background(backgroundColor)
                                .BorderBottom(1)
                                .BorderColor(Border)
                                .PaddingHorizontal(8)
                                .PaddingVertical(11)
                                .AlignRight()
                                .Text($"{item.UnitPrice:C}")
                                .FontSize(10)
                                .FontColor(TextDark);

                            // Line total
                            table
                                .Cell()
                                .Background(backgroundColor)
                                .BorderBottom(1)
                                .BorderColor(Border)
                                .PaddingHorizontal(12)
                                .PaddingVertical(11)
                                .AlignRight()
                                .Text($"{item.LineTotal:C}")
                                .FontSize(10)
                                .Bold()
                                .FontColor(TextDark);

                            isEvenRow = !isEvenRow;
                        }
                    });

                // Totals
                column
                    .Item()
                    .PaddingTop(28)
                    .Row(row =>
                    {
                        // Left side message
                        row.RelativeItem()
                            .AlignBottom()
                            .Column(message =>
                            {
                                message
                                    .Item()
                                    .Text("Thank you for choosing Mechanic Shop.")
                                    .FontSize(10)
                                    .Bold()
                                    .FontColor(TextDark);

                                message
                                    .Item()
                                    .PaddingTop(4)
                                    .Text("We appreciate your business.")
                                    .FontSize(9)
                                    .FontColor(TextMedium);
                            });

                        // Right side totals
                        row.ConstantItem(235)
                            .Background(Surface)
                            .Border(1)
                            .BorderColor(Border)
                            .Padding(16)
                            .Column(totals =>
                            {
                                totals
                                    .Item()
                                    .Row(totalRow =>
                                    {
                                        totalRow
                                            .RelativeItem()
                                            .Text("Subtotal")
                                            .FontSize(10)
                                            .FontColor(TextMedium);

                                        totalRow
                                            .AutoItem()
                                            .Text($"{invoice.SubTotal:C}")
                                            .FontSize(10)
                                            .FontColor(TextDark);
                                    });

                                totals
                                    .Item()
                                    .PaddingTop(8)
                                    .Row(totalRow =>
                                    {
                                        totalRow
                                            .RelativeItem()
                                            .Text("Tax")
                                            .FontSize(10)
                                            .FontColor(TextMedium);

                                        totalRow
                                            .AutoItem()
                                            .Text($"{invoice.TaxAmount:C}")
                                            .FontSize(10)
                                            .FontColor(TextDark);
                                    });

                                if (invoice.DiscountAmount > 0)
                                {
                                    totals
                                        .Item()
                                        .PaddingTop(8)
                                        .Row(totalRow =>
                                        {
                                            totalRow
                                                .RelativeItem()
                                                .Text("Discount")
                                                .FontSize(10)
                                                .FontColor(TextMedium);

                                            totalRow
                                                .AutoItem()
                                                .Text($"-{invoice.DiscountAmount:C}")
                                                .FontSize(10)
                                                .FontColor("#DC2626");
                                        });
                                }

                                // Final total
                                totals
                                    .Item()
                                    .PaddingTop(12)
                                    .BorderTop(1.5f)
                                    .BorderColor(Primary)
                                    .PaddingTop(12)
                                    .Row(totalRow =>
                                    {
                                        totalRow
                                            .RelativeItem()
                                            .Text("TOTAL")
                                            .FontSize(13)
                                            .Bold()
                                            .FontColor(Primary);

                                        totalRow
                                            .AutoItem()
                                            .Text($"{invoice.Total:C}")
                                            .FontSize(16)
                                            .Bold()
                                            .FontColor(AccentDark);
                                    });
                            });
                    });

                // Bottom accent
                column.Item().PaddingTop(30).AlignCenter().Width(70).Height(3).Background(Accent);
            });
        };

    private Action<IContainer> BuildFooter() =>
        footer =>
        {
            footer
                .BorderTop(1)
                .BorderColor(Border)
                .PaddingTop(10)
                .Row(row =>
                {
                    row.RelativeItem()
                        .Text("MECHANIC SHOP • AUTOMOTIVE SERVICE & REPAIR")
                        .FontSize(8)
                        .FontColor(TextMedium);

                    row.AutoItem()
                        .Text(text =>
                        {
                            text.Span("Generated ").FontSize(8).FontColor(TextMedium);

                            text.Span($"{DateTime.UtcNow:MMM dd, yyyy • HH:mm} UTC")
                                .FontSize(8)
                                .SemiBold()
                                .FontColor(TextMedium);
                        });
                });
        };

    private static string GetStatusColor(string status)
    {
        return status.ToLowerInvariant() switch
        {
            "paid" => "#15803D",
            "scheduled" => "#B45309",
            "overdue" => "#B91C1C",
            "cancelled" => "#6B7280",
            _ => "#6B7280",
        };
    }

    private static string GetStatusBackgroundColor(string status)
    {
        return status.ToLowerInvariant() switch
        {
            "paid" => "#DCFCE7",
            "scheduled" => "#FEF3C7",
            "overdue" => "#FEE2E2",
            "cancelled" => "#F3F4F6",
            _ => "#F3F4F6",
        };
    }
}
