using System;
using System.Linq;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SigeMobile.Model;
using SigeTools.Helpers;

namespace SigeTools.Services
{
    public interface IPdfService
    {
        byte[] GenerateQuotePdf(Orden ordenApp);
    }
    public class PdfService:IPdfService
    {
        public PdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }
        int bodySize = 13;
        int titleSize = 15;
        private CompanyInfo companyInfo;

        public byte[] GenerateQuotePdf(Orden ordenApp)
        {
             companyInfo = JsonConvert.DeserializeObject<CompanyInfo>(Settings.CompanyInfo);
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    ConfigurePage(page);

                    page.Header().Element((header)=>ComposeHeader(header,ordenApp));
                    page.Content().Element((body) => ComposeContent(body, ordenApp));
                });
            }).GeneratePdf();

        }

        void ConfigurePage(PageDescriptor page)
        {
            page.Size(PageSizes.A3);
            page.Margin(2, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(style => style.FontSize(16));
        }

        void ComposeHeader(IContainer container,Orden orden)
        {
           
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontSize(titleSize);
            container.Table((table) =>
            {
                table.ColumnsDefinition((columndefinition) =>
                {
                    columndefinition.RelativeColumn();
                    columndefinition.RelativeColumn();
                });
                if (companyInfo.CompanyLogo.Any() )
                {
                    table.Cell()
                        .Row(1)
                        .Height(70)
                        .Image(companyInfo.CompanyLogo)
                        .FitHeight()
                        .FitArea();
                }
    
                table.Cell()
                    .Row(2)
                    .Text(companyInfo.CompanyName).Style(titleStyle);

                table.Cell().Row(3).Text("Santo Domingo").FontSize(titleSize);
                table.Cell().Row(4).Text("Dominican Republic").FontSize(titleSize);
                table.Cell().Row(5).Text(companyInfo.CompanyPhone).FontSize(titleSize);
                //companyInfo
                table.Cell().Row(2).RowSpan(4).Column(2).AlignRight().Text("CUOTA").FontSize(40);
                table.Cell().Row(6).Column(2).AlignRight().Text($"# QT-{orden.OrdenId.PadLeft(5,'0')??""}").FontSize(titleSize);
            });
        }

        void ComposeContent(IContainer container, Orden order)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(1);

                column.Item()
                    .Row(row => { row.RelativeItem().Text("Cotizar A:").FontSize(titleSize); });
                column.Item().Row((row) =>
                {
                    row.RelativeItem().Text(order.CustomerName).SemiBold().FontSize(titleSize);
                    row.RelativeItem().AlignRight().Text(DateTime.Now.ToString("dd MMM yyyy")).FontSize(titleSize);
                });
                column.Item().Height(15);
                column.Spacing(1);
                column.Item().Element((t) => ComposeTable(t, order));
                column.Item().Element((element) =>
                {
                    element.Table((table) =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.ConstantColumn(120);
                        });

                        var totalPrice = order.Lineas.Sum(x => x.Total);
                        table.Cell().Row(1).Column(2).AlignRight().PaddingVertical(10).Text($"Sub Total: ")
                            .FontSize(bodySize);
                        table.Cell().Row(1).Column(3).Element((a) => a
                                .PaddingVertical(5)
                                .PaddingHorizontal(10)
                                .AlignRight()
                                .AlignMiddle()).Text($"{totalPrice:N2}")
                            .FontSize(bodySize);

                        table.Cell().Row(3).Column(2).Element((b) => b
                            .Background("#F5F4F3")
                            .PaddingVertical(5)
                            .PaddingHorizontal(10)
                            .AlignRight()

                            .AlignMiddle()).PaddingVertical(10).Text($"Total: ").SemiBold().FontSize(bodySize);
                        table.Cell().Row(3).Column(3).Element((s) => s
                            .Background("#F5F4F3")
                            .PaddingVertical(10)
                            .PaddingHorizontal(10)
                            .AlignRight()

                            .AlignMiddle()).Text($"{totalPrice:N2}").FontSize(bodySize).SemiBold();
                    });
                });



            });
        }

        void ComposeTable(IContainer tableCompose, Orden order)
        {

            tableCompose.MinimalBox().Border(0).Table(table =>
            {
     
      

                // step 1
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(50);
                    columns.RelativeColumn(5);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.ConstantColumn(120);
                });

                // step 2
                table.Header(header =>
                {

                    header.Cell().Element(DefaultCellStyle2).Text("#").FontSize(bodySize).FontColor(Colors.White);
                    header.Cell().Element((cellstyle) =>
                    {
                        var style = CellStyleHeader(cellstyle);
                        style.AlignLeft();
                        return style;
                    }).Text("Descripción del Producto").FontSize(bodySize).FontColor(Colors.White);
                    header.Cell().Element(CellStyleHeader).AlignRight().Text("Cantidad").FontSize(bodySize)
                        .FontColor(Colors.White);
                    header.Cell().Element(CellStyleHeader).AlignRight().Text("Precio").FontSize(bodySize)
                        .FontColor(Colors.White);
                    header.Cell().Element((cell) =>
                    {
                        var style = CellStyleHeader(cell);
                        style.AlignRight();
                        return style;
                    }).AlignRight().Text("Total").FontSize(bodySize).FontColor(Colors.White);


                });

                // step 3
                foreach (var item in order.Lineas)
                {
                    table.Cell().Element(CellStyleBody).Text(order.Lineas.IndexOf(item) + 1)
                        .FontSize(bodySize);
                    ;
                    table.Cell().Element((cellstyle) =>
                    {
                        var style = CellStyleBody(cellstyle);
                        style.AlignLeft();
                        return style;
                    }).Text((text) =>
                    {
                        text.Span($"{item.Descripcion}\n").FontSize(bodySize);
                        ;
                        text.Span($"{item.ProdutoId}").FontColor(Colors.Grey.Lighten1).FontSize(bodySize);
                        ;
                    });
                    table.Cell().Element(CellStyleBody).AlignRight().Text($"{item.Cantidad:#,0}")
                        .FontSize(bodySize);
                    table.Cell().Element(CellStyleBody).AlignRight().Text($"{item.Precio:N2}")
                        .FontSize(bodySize);
                    ;
                    table.Cell().Element((style) =>
                        {
                            var cell = CellStyleBody(style);
                            cell.AlignRight();
                            return cell;
                        }).AlignRight().Text($"{item.Total:N2}")
                        .FontSize(bodySize);
                    ;


                }

            });
        }
        IContainer DefaultCellStyle2(IContainer container)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .Background("#3C3D3A")
                .PaddingVertical(5)

                .AlignCenter()
                .AlignMiddle();
        }
        IContainer DefaultCellStyle(IContainer container, string backgroundColor)
        {
            return container
                .Border(1)
                .BorderColor(Colors.Grey.Lighten1)
                .Background("#3C3D3A")
                .PaddingVertical(5)
                .PaddingHorizontal(10)
                .AlignCenter()
                .AlignMiddle();
        }

        IContainer CellStyleHeader(IContainer container) => DefaultCellStyle(container, Colors.Grey.Lighten3);

        static IContainer CellStyleBody(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten1)
                .PaddingVertical(5)
                .PaddingHorizontal(10)
                .AlignCenter()
                .AlignMiddle();
        }
    }
}


