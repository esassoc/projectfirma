/*-----------------------------------------------------------------------
<copyright file="DhtmlxGridHtmlHelpers.cs" company="Environmental Science Associates">
Copyright (c) Environmental Science Associates. All rights reserved.
<author>Environmental Science Associates</author>
</copyright>

<license>
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

Source code is available upon request via <support@sitkatech.com>.
</license>
-----------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using LtInfo.Common.BootstrapWrappers;
using LtInfo.Common.ModalDialog;

namespace LtInfo.Common.AgGridWrappers
{
    /// <summary>
    ///     Helper class for DhtmlxGrid expects following content to be set up in local project
    ///     /Content/css/dhtmlxgrid_skin.css
    ///     /Content/img/bg-edit-single.png
    ///     /Content/img/bg-delete-single.png
    /// </summary>
    public static class AgGridHtmlHelpers
    {
        public static readonly HtmlString PlusIconBootstrap = BootstrapHtmlHelpers.MakeGlyphIcon("glyphicon-plus-sign gi-1x blue");
        public static readonly HtmlString UndoIconBootstrap = BootstrapHtmlHelpers.MakeGlyphIcon("glyphicon-regular undo gi-1x blue");
        public static readonly HtmlString EditIconBootstrap = BootstrapHtmlHelpers.MakeGlyphIcon("glyphicon-edit gi-1x blue");
        public static readonly HtmlString DeleteIconBootstrap = BootstrapHtmlHelpers.MakeGlyphIcon("glyphicon-trash gi-1x blue");
        public static readonly HtmlString OkCircleIconBootstrap = BootstrapHtmlHelpers.MakeGlyphIconWithHiddenText("glyphicon-ok-circle gi-1x blue", "Yes");

        public const string EditIcon = "<img src=\"/Content/img/bg-edit-single.png\" />";
        public const string DeleteIcon = "<img src=\"/Content/img/bg-delete-single.png\" />";
        public const string DeleteIconGrey = "<img src=\"/Content/img/bg-delete-single-grey.png\" />";

        public const string Skin = "mm";
        public const int SkinRowHeight = 30;

        /// <summary>
        /// Only public for unit testing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gridSpec"></param>
        /// <param name="gridName"></param>
        /// <param name="optionalGridDataUrl"></param>
        /// <param name="styleString"></param>
        /// <returns></returns>
        public static HtmlString AgGrid<T>(GridSpec<T> gridSpec, string gridName, string optionalGridDataUrl, string styleString)
        {
            return new HtmlString(AgGridImpl(gridSpec, gridName, optionalGridDataUrl, styleString, AgGridResizeType.None, false));
        }

        public static string AgGridImpl<T>(GridSpec<T> gridSpec, string gridName, string optionalGridDataUrl, string styleString)
        {
            return AgGridImpl(gridSpec, gridName, optionalGridDataUrl, styleString, AgGridResizeType.None, false);
        }

        public static string AgGridImpl<T>(GridSpec<T> gridSpec, string gridName, string optionalGridDataUrl, string styleString, AgGridResizeType agGridResizeType, bool showGridSettingsButtons)
        {


            const string template = @"    <!-- The div that will host the grid. ag-theme-alpine is the theme. -->
    <!-- The grid will be the size that this element is given. -->
    {12}
    <div class=""row"">
        <div class=""col-md-5""><span id=""{0}RowCountText""></span> <a id=""{0}ClearFilters"" style=""display: none"" href=""javascript: void(0);"" onclick=""{0}ClearFilters()"">(clear filters)</a></div>
        <div class=""col-md-7 text-right gridDownloadContainer"">{9}<span>{10}</span><button class=""excelbutton"" href=""javascript: void(0);""  onclick=""{0}OnBtnExport()"">Download Table</button>{8}</div>
    </div>
    <div id=""{0}DivID"" aria-label=""{0}"" class=""ag-theme-alpine"" style=""{6}"" tabIndex=""0""></div>
    <script type=""text/javascript"">

            function {0}ClearFilters(){{
                {0}GridOptionsApi.setFilterModel(null);
                document.getElementById(""{0}ClearFilters"").style.display = ""none"";
            }}

            function {0}OnBtnExport() {{
                {0}GridOptionsApi.exportDataAsCsv({{ fileName: '{0}' + 'Export', columnKeys: [{11}] }});
            }}

            function {0}GetValuesFromCheckedGridRows(valueColumnName, returnListName) {{
                const selectedData = {0}GridOptionsApi.getSelectedRows();
                var values = selectedData.map((row) => row[valueColumnName]);
                var returnList = new Object();
                returnList[returnListName] = values
                return returnList;
            }}

            function {0}GetValueFromSelectedGridRow(valueColumnName) {{
                const selectedData = {0}GridOptionsApi.getSelectedRows();
                var values = selectedData.map((row) => row[valueColumnName]);
                return values[0];
            }}

            function {0}ResizeGridWithVerticalFill(){{
                var top = getOffsetTop(document.getElementById(""{0}DivID""));
                gridHeight =getGridHeight(top);
                document.getElementById(""{0}DivID"").style.height = gridHeight+""px"";
            }}
            addEventListener(""resize"", (event) => {{ {4} }}); ; // insert method to resize grid vertically if grid resize type is VerticalResizableHorizontalAutoFit

            function {0}MakeGridVerticallyResizable() {{
                document.getElementById(""{0}DivID"").style.resize = ""vertical"";
                document.getElementById(""{0}DivID"").style.overflow = ""auto"";
            }}
            {5} // insert method to make grid vertically resizable if the grid resize type is VerticalResizableHorizontalAutoFit

            function {0}GeneratePinnedBottomData(){{
                // generate a row-data with null values
                var result = {{}};

                {0}GridOptionsApi.getAllGridColumns().forEach(item => {{
                    result[item.colId] = null;
                    if(item.colDef.aggregationType === ""total"") {{
                        columnsWithAggregation.push(item.colId);
                    }}
                }});
                var columnsWithAggregation = [{7}];
                if(columnsWithAggregation.length === 0){{ return false;}}
                return {0}CalculatePinnedBottomData(result, columnsWithAggregation);
            }}
            function {0}CalculatePinnedBottomData(target, columnsWithAggregation){{
                //console.log(target);
                
                columnsWithAggregation.forEach(element => {{
                  //console.log('element', element);
                    {0}GridOptionsApi.forEachNodeAfterFilter((rowNode) => {{
                        if (rowNode.data[element]){{
                            if(target[element]){{
                                target[element] = (Number.parseFloat(target[element]) + Number.parseFloat(rowNode.data[element]));
                            }}else{{
                                target[element] = Number.parseFloat(rowNode.data[element]);
                            }}
                            
                        }}
                    }});
                    if (target[element]){{
                        target[element] = target[element].toFixed(2);
                    }}
                }})

                return target;
            }}


        // Function to demonstrate calling grid's API
        function {0}Deselect(){{
            {0}GridOptionsApi.deselectAll()
        }}

        function {0}LoadGridData(url){{
                    // Fetch data from server
            fetch(url)
            .then(response => response.json())
            .then(data => {{
                // load fetched data into grid
                {0}GridOptionsApi.setGridOption('rowData', data);
                {0}TotalRowCount = data.length;
                document.getElementById(""{0}RowCountText"").innerText=""Currently Viewing ""+{0}GridOptionsApi.getDisplayedRowCount()+ "" out of "" + {0}TotalRowCount + "" {3}""; 
                {4}; // insert method to resize grid vertically if grid resize type is VerticalResizableHorizontalAutoFit
                var {0}PinnedBottomData = {0}GeneratePinnedBottomData();
                if({0}PinnedBottomData){{
                    {0}GridOptionsApi.setGridOption('pinnedBottomRowData',[{0}PinnedBottomData]);
                }}
                {0}GridOptionsApi.setGridOption('loading', false);
                loadGridState({0}GridOptionsApi, '{0}', false);
            }});
           
        }}

        // Grid Options are properties passed to the grid
        const {0}GridOptions = {{

          // each entry here represents one column
          columnDefs: [
            {2}
          ],

          // default col def properties get applied to all columns
          defaultColDef: {{sortable: true, filter: true, floatingFilter:true, wrapHeaderText: true }},
          rowSelection: 'multiple', // allow rows to be selected
          animateRows: true, // have rows animate to new positions when sorted

          // Accessibility options
          suppressCellFocus: false,
          ensureDomOrder: true,
          suppressRowClickSelection: false,

          dataTypeDefinitions: {{
            dateString: getDateStringDataTypeDefinition()
          }},


          onCellFocused: function(event) {{
            // Focus the first link in the cell if present
            if (event && event.rowIndex != null && event.column) {{
              var cell = document.querySelector(
                '#{0}DivID [row-index=""' + event.rowIndex + '""] [col-id=""' + event.column.getId() + '""]'
              );
              if (cell) {{
                var link = cell.querySelector('a');
                if (link) {{
                  link.focus();
                }}
              }}
            }}
          }},

          onFilterChanged: function() {{
            document.getElementById(""{0}RowCountText"").innerText=""Currently Viewing ""+{0}GridOptionsApi.getDisplayedRowCount()+ "" out of "" + {0}TotalRowCount + "" {3}"";
            if(Object.keys({0}GridOptionsApi.getFilterModel()).length !== 0){{
                document.getElementById(""{0}ClearFilters"").style.display = ""inline-block"";
            }}
            var {0}PinnedBottomData = {0}GeneratePinnedBottomData();
            if({0}PinnedBottomData){{
                {0}GridOptionsApi.setGridOption('pinnedBottomRowData',[{0}PinnedBottomData]);
            }}
          }},

          // example event handler
          onCellClicked: params => {{
            //debugger;
            //console.log('cell was clicked', params)

          }},
          // Add aria-rowindex for accessibility
          getRowAriaAttributes: params => {{
            // AG Grid uses zero-based row index, but aria-rowindex should be 1-based and include header row
            return {{ 'aria-rowindex': params.node ? (params.node.rowIndex + 2) : 1 }};
          }},
          // Add aria-colindex for accessibility
          getCellAriaAttributes: params => {{
            // AG Grid uses zero-based col index, but aria-colindex should be 1-based
            return {{ 'aria-colindex': params.column ? (params.column.getInstanceId ? (params.column.getInstanceId() + 1) : (params.column.getColId ? (params.column.getColId() + 1) : 1)) : 1 }};
          }}
        }};

        // get div to host the grid
        const {0}GridDiv = document.getElementById(""{0}DivID"");
        // new grid instance, passing in the hosting DIV and Grid Options
        var {0}GridOptionsApi = agGrid.createGrid({0}GridDiv, {0}GridOptions);
        var {0}TotalRowCount = 0;
        {0}LoadGridData(""{1}"");
        
    </script>";


            //{{ field: ""Watershed"" }},
            //{{ field: ""NumofProjects"" }}
            var columnDefinitionStringBuilder = new StringBuilder();
            var columnsWithAggregationStringBuilder = new StringBuilder();
            var columnsForCsvOutput = new List<string>();
            var isFirstLoop = true;
            foreach (var columnSpec in gridSpec)
            {
                if (isFirstLoop)
                {
                    isFirstLoop = false;
                }
                else
                {
                    columnDefinitionStringBuilder.Append(",");
                }

                columnDefinitionStringBuilder.Append("{ "); //open this column spec
                // Column names can come from admin-configurable field definition labels, and are emitted here inside
                // JavaScript string literals. JS-string encoding keeps a quote (or a "</script>") in a label from
                // breaking out of the literal; the escapes decode back to the original characters at runtime, so the
                // resulting field key and header text are unchanged.
                columnDefinitionStringBuilder.AppendFormat("\"field\": \"{0}\"", HttpUtility.JavaScriptStringEncode(columnSpec.ColumnNameForJavascript));

                if (columnSpec.AgGridColumnSortType.SortingType.Equals("htmlstring"))
                {
                    // remove html for sorting
                    columnDefinitionStringBuilder.Append(
                        ", \"comparator\":  (valueA, valueB, nodeA, nodeB, isDescending) => { var valueANoHtml = removeHtmlFromString(valueA); var valueBNoHtml = removeHtmlFromString(valueB); if (valueANoHtml == valueBNoHtml) return 0; return (valueANoHtml > valueBNoHtml) ? 1 : -1; }");
                }

                columnDefinitionStringBuilder.AppendFormat(", \"headerName\": \"{0}\"", HttpUtility.JavaScriptStringEncode(columnSpec.ColumnNameInnerText));

                columnDefinitionStringBuilder.Append(
                    ", \"headerComponentParams\": { \"template\": \"<div class=\\\"ag-cell-label-container\\\">");
                columnDefinitionStringBuilder.Append(
                    "<span data-ref=\\\"eMenu\\\" class=\\\"ag-header-icon ag-header-cell-menu-button\\\"></span>");
                columnDefinitionStringBuilder.Append(
                    "<span data-ref=\\\"eFilterButton\\\" class=\\\"ag-header-icon ag-header-cell-filter-button\\\"></span>");
                columnDefinitionStringBuilder.Append(
                    "<div data-ref=\\\"eLabel\\\" class=\\\"ag-header-cell-label\\\">");
                columnDefinitionStringBuilder.Append(
                    "<span data-ref=\\\"eSortOrder\\\" class=\\\"ag-header-icon ag-sort-order\\\" ></span>");
                columnDefinitionStringBuilder.Append(
                    "<span data-ref=\\\"eSortAsc\\\" class=\\\"ag-header-icon ag-sort-ascending-icon\\\" ></span>");
                columnDefinitionStringBuilder.Append(
                    "<span data-ref=\\\"eSortDesc\\\" class=\\\"ag-header-icon ag-sort-descending-icon\\\" ></span>");
                columnDefinitionStringBuilder.Append(
                    "<span data-ref=\\\"eSortNone\\\" class=\\\"ag-header-icon ag-sort-none-icon\\\" ></span>");
                columnDefinitionStringBuilder.Append(
                    "<span data-ref=\\\"eText2\\\" class=\\\"ag-header-cell-text\\\">");
                // 7/31/2023 TK - Not sure if I like this, it works with the current setup of helpers but feels a bit hacky
                //todo: come up with a better method to get field definition popups in header cells
                columnDefinitionStringBuilder.AppendFormat("{0}</span>", columnSpec.ColumnName.ToJSON());
                columnDefinitionStringBuilder.Append(
                    "<span data-ref=\\\"eFilter\\\" class=\\\"ag-header-icon ag-filter-icon\\\"></span>");
                columnDefinitionStringBuilder.Append("</div></div>\" }"); //close headerComponentParams object

                bool resizable =
                    !(columnSpec.GridWidth <=
                      35); // most cols with a width <= 35px are icon buttons (like edit, delete, download fact sheet)  
                columnDefinitionStringBuilder.AppendFormat(", \"resizable\": {0}", resizable.ToString().ToLower());
                bool
                    autoHeaderHeight =
                        !(columnSpec.GridWidth <=
                          35); // most cols with a width <= 35px are icon buttons (like edit, delete, download fact sheet) and we do not want to adjust the header height for these
                columnDefinitionStringBuilder.AppendFormat(", \"autoHeaderHeight\": {0}",
                    autoHeaderHeight.ToString().ToLower());

                if (!resizable)
                {
                    // hide the ellipsis 
                    columnDefinitionStringBuilder.Append(", \"cellStyle\": { \"text-overflow\" : \"clip\"}");
                }

                //hide column or set grid width
                if (columnSpec.GridWidth == 0)
                {
                    columnDefinitionStringBuilder.Append(", \"hide\": true");

                }
                else if (columnSpec.GridWidth <= 35)
                {
                    columnDefinitionStringBuilder.AppendFormat(", \"initialWidth\": {0}", columnSpec.GridWidth);
                }
                else
                {
                    columnDefinitionStringBuilder.AppendFormat(", \"initialWidth\": {0}",
                        columnSpec.GridWidth +
                        30); // 8/8/2023 SB add to the width instead of editing every hard coded column in every GridSpec class
                }

                if (columnSpec.GridWidthFlex > 0)
                {
                    columnDefinitionStringBuilder.AppendFormat(", \"flex\": {0}", columnSpec.GridWidthFlex);
                    columnDefinitionStringBuilder.AppendFormat(", \"minWidth\": {0}", columnSpec.GridWidth + 30);
                }

                //only add columns that are not buttons or hidden to the csv output
                if (columnSpec.GridWidth > 35)
                {
                    columnsForCsvOutput.Add(columnSpec.ColumnNameForJavascript);
                }



                switch (columnSpec.AgGridColumnFilterType)
                {
                    case AgGridColumnFilterType.None:
                        columnDefinitionStringBuilder.Append(", \"filter\": false");
                        break;
                    case AgGridColumnFilterType.SelectFilterStrict:
                    case AgGridColumnFilterType.SelectFilterHtmlStrict:
                        //columnDefinitionStringBuilder.Append(", \"floatingFilterComponent\": DropdownFloatingFilterComponent");
                        //columnDefinitionStringBuilder.AppendFormat(", \"floatingFilterComponentParams\": {{suppressFilterButton: false, field: \"{0}\"}}", columnSpec.ColumnNameForJavascript);
                        columnDefinitionStringBuilder.Append(", \"filter\": DropdownFilterComponent");
                        columnDefinitionStringBuilder.Append(", \"valueFormatter\": HtmlRemovalFormatter");
                        break;
                    case AgGridColumnFilterType.FormattedNumeric:
                    case AgGridColumnFilterType.Numeric:
                        columnDefinitionStringBuilder.Append(", \"filter\": \"agNumberColumnFilter\"");
                        break;
                    case AgGridColumnFilterType.DateRange:
                        columnDefinitionStringBuilder.Append(", \"filter\": \"agDateColumnFilter\"");
                        columnDefinitionStringBuilder.Append(
                            ", \"comparator\": dateStringComparator"); //comparator: dateComparator
                        // columnDefinitionStringBuilder.Append(", \"cellDataType\": \"dateString\"");
                        break;
                    case AgGridColumnFilterType.Html:
                        columnDefinitionStringBuilder.Append(", \"filter\": \"agTextColumnFilter\"");
                        columnDefinitionStringBuilder.Append(
                            ", \"filterParams\": { \"textMatcher\": ({ filterOption, value, filterText }) => htmlFilterTextMatcher( filterOption, value, filterText)  }");
                        columnDefinitionStringBuilder.Append(", \"valueFormatter\": HtmlRemovalFormatter");
                        //6/28/24 TK - This causes sorting to be very slow, so we are removing it for now
                        //columnDefinitionStringBuilder.Append(", \"comparator\": HtmlRemovalSorting");
                        break;
                    case AgGridColumnFilterType.Text:
                        columnDefinitionStringBuilder.Append(", \"filter\": \"agTextColumnFilter\"");
                        break;
                    case AgGridColumnFilterType.HtmlLinkJson:
                        columnDefinitionStringBuilder.Append(", \"filter\": \"agTextColumnFilter\"");
                        columnDefinitionStringBuilder.Append(
                            ", \"filterParams\": { \"textMatcher\": ({ filterOption, value, filterText }) => htmlLinkJsonFilterTextMatcher( filterOption, value, filterText)  }");
                        columnDefinitionStringBuilder.Append(", \"cellRenderer\": HtmlLinkJsonRenderer");
                        columnDefinitionStringBuilder.Append(", \"valueFormatter\": HtmlLinkJsonFormatter");
                        columnDefinitionStringBuilder.Append(", \"comparator\": JsonDisplayTextSorting");
                        break;
                    case AgGridColumnFilterType.HtmlLinkJsonWithNoFilter:
                        columnDefinitionStringBuilder.Append(", \"filter\": false");
                        columnDefinitionStringBuilder.Append(", \"cellRenderer\": HtmlLinkJsonRenderer");
                        columnDefinitionStringBuilder.Append(", \"valueFormatter\": HtmlLinkJsonFormatter");
                        columnDefinitionStringBuilder.Append(", \"sortable\": false");
                        break;
                    case AgGridColumnFilterType.HtmlLinkListJson:
                        columnDefinitionStringBuilder.Append(", \"filter\": HtmlLinkListDropdownFilterComponent");
                        //columnDefinitionStringBuilder.Append(", \"filterParams\": { \"textMatcher\": ({ filterOption, value, filterText }) => htmlLinkJsonFilterTextMatcher( filterOption, value, filterText)  }");
                        columnDefinitionStringBuilder.Append(", \"cellRenderer\": HtmlLinkListJsonRenderer");
                        columnDefinitionStringBuilder.Append(", \"valueFormatter\": HtmlLinkListJsonFormatter");
                        //columnDefinitionStringBuilder.Append(", \"comparator\": JsonDisplayTextSorting");
                        break;
                    default:
                        break;
                }

                switch (columnSpec.AgGridColumnFormatType)
                {
                    case AgGridColumnFormatType.Currency:
                        columnDefinitionStringBuilder.Append(", \"valueFormatter\": currencyFormatter");
                        break;
                    case AgGridColumnFormatType.Integer:
                        columnDefinitionStringBuilder.Append(", \"valueFormatter\": integerFormatter");
                        // columnDefinitionStringBuilder.Append(", \"cellDataType\": \"number\"");
                        break;
                    case AgGridColumnFormatType.Decimal:
                        columnDefinitionStringBuilder.Append(", \"valueFormatter\": decimalFormatter");
                        break;
                    default:
                        break;
                }

                switch (columnSpec.GridColumnAggregationType)
                {
                    case AgGridColumnAggregationType.Total:
                        columnsWithAggregationStringBuilder.Append(columnsWithAggregationStringBuilder.Length > 0
                            ? ", "
                            : string.Empty);
                        columnsWithAggregationStringBuilder.AppendFormat("\"{0}\"", columnSpec.ColumnNameForJavascript);
                        break;
                    default:
                        break;
                }

                switch (columnSpec.AgGridColumnAlignType)
                {
                    case AgGridColumnAlignType.Right:
                        columnDefinitionStringBuilder.Append(", \"cellClass\": \"ag-right-aligned-cell\"");
                        break;
                    default:
                        break;
                }

                if (columnSpec.AgGridColumnDataType == AgGridColumnDataType.ReadOnlyHtmlText)
                {
                    columnDefinitionStringBuilder.Append(
                        ", \"cellRenderer\": function(params) { return params.value ? params.value: ''; } ");
                }

                if (columnSpec.AgGridColumnDataType == AgGridColumnDataType.Checkbox)
                {
                    columnDefinitionStringBuilder.Append(", \"checkboxSelection\": true");
                }


                columnDefinitionStringBuilder.Append(" }"); //close this column spec
            }

            var resizeGridFunction = agGridResizeType == AgGridResizeType.VerticalFillHorizontalAutoFit
                ? $"{gridName}ResizeGridWithVerticalFill()"
                : string.Empty;

            var makeVerticalResizable = agGridResizeType == AgGridResizeType.VerticalResizableHorizontalAutoFit
                ? $"{gridName}MakeGridVerticallyResizable()"
                : string.Empty;

            if (string.IsNullOrWhiteSpace(styleString) || !styleString.ToLower().Contains("height"))
            {
                styleString = styleString == null ? "height: 500px;" : styleString + "; height: 500px";
            }

            var customDownloadLink = CreateFullDatabaseExcelDownloadIconHtml(gridName, gridSpec.CustomExcelDownloadUrl,
                gridSpec.CustomExcelDownloadLinkText ?? "Download Full Database");

            var generateReportsIconHtml = CreateGenerateReportUrlHtml(gridName, gridSpec.GenerateReportModalDialogForm);
            var tagIconHtml = CreateTagUrlHtml(gridName, gridSpec.BulkTagModalDialogForm);
            var arbitraryHtml = CreateArbitraryHtml(gridSpec.ArbitraryHtml, "    ");
            var additionalIcons =
                $"{(!string.IsNullOrWhiteSpace(arbitraryHtml) ? $"<span>{arbitraryHtml}</span>" : string.Empty)}{(!string.IsNullOrWhiteSpace(generateReportsIconHtml) ? $"<span>{generateReportsIconHtml}</span>" : string.Empty)}{(!string.IsNullOrWhiteSpace(tagIconHtml) ? $"<span>{tagIconHtml}</span>" : string.Empty)}";
            var columnsForCsvDownloadString = string.Join(",", columnsForCsvOutput.Select(x => $"\"{x}\""));

            var createEntityHtml = CreateCreateUrlHtml(gridSpec.CreateEntityUrl, gridSpec.CreateEntityUrlClass,
                gridSpec.CreateEntityModalDialogForm, gridSpec.CreateEntityActionPhrase, gridSpec.ObjectNameSingular);


            var gridSettingsButtonsHtml = CreateGridSettingsButtonsHtml(gridName, showGridSettingsButtons);

            return String.Format(template,
                gridName, //0
                optionalGridDataUrl, //1
                columnDefinitionStringBuilder, //2 
                // ObjectNamePlural can contain user-entered data (e.g. a person's name) and is emitted inside a
                // JavaScript string literal in the template below, so it needs JS-string encoding, not HTML encoding.
                // JavaScriptStringEncode also escapes < and > as </>, which prevents a "</script>" in the
                // value from closing the inline script block.
                HttpUtility.JavaScriptStringEncode(gridSpec.ObjectNamePlural), //3
                resizeGridFunction, //4
                makeVerticalResizable, //5
                styleString, //6
                columnsWithAggregationStringBuilder, //7
                customDownloadLink, //8
                additionalIcons, //9
                createEntityHtml, //10
                columnsForCsvDownloadString, //11
                gridSettingsButtonsHtml); //12 total

        }



        /// <summary>
        /// Creates the download to excel icon
        /// If exceldownloadurl is null or empty, will return empty string
        /// </summary>
        /// <param name="gridName"></param>
        /// <param name="excelDownloadUrl"></param>
        /// <param name="linkText"></param>
        /// <param name="hoverText"></param>
        /// <returns></returns>
        public static string CreateCustomExcelDownloadIconHtml(string gridName, string excelDownloadUrl, string linkText, string hoverText)
        {
            if (!String.IsNullOrWhiteSpace(excelDownloadUrl))
            {
                return $@"<a class=""excelbutton"" id=""{
                        gridName
                    }ExcelDownloadLink"" href=""{excelDownloadUrl}"" title=""{hoverText}"">{linkText}</a>";
            }
            return String.Empty;
        }

        public static string CreateFullDatabaseExcelDownloadIconHtml(string gridName, string excelDownloadUrl, string excelDownloadLinkText)
        {
            return CreateCustomExcelDownloadIconHtml(gridName, excelDownloadUrl, excelDownloadLinkText, "Download the full database as an Excel file");
        }

        /// <summary>
        /// Creates the Generate Reports icon
        /// </summary>
        /// <param name="gridName"></param>
        /// <param name="modalDialogForm"></param>
        /// <returns></returns>
        public static string CreateGenerateReportUrlHtml(string gridName, SelectProjectsModalDialogForm modalDialogForm)
        {
            if (modalDialogForm == null)
            {
                return string.Empty;
            }
            var tagIconHtml =
                $"<span style=\"margin-right:5px\">{BootstrapHtmlHelpers.MakeGlyphIcon("glyphicon-file")}</span>";
            var getProjectIDFunctionString =
                $"function() {{ return {gridName}GetValuesFromCheckedGridRows('{modalDialogForm.ValueColumnName}', '{modalDialogForm.ReturnListName}'); }}";

            return ModalDialogFormHelper.ModalDialogFormLink($"{tagIconHtml} Generate Reports",
                modalDialogForm.DialogUrl,
                modalDialogForm.DialogTitle,
                ModalDialogFormHelper.DefaultDialogWidth,
                "Generate",
                "Close",
                new List<string>(),
                "closeModalAsReportGeneratesInBackground",
                getProjectIDFunctionString,
                true).ToString();

        }

        /// <summary>
        /// Creates the Tag Checked Projects icon
        /// </summary>
        /// <param name="gridName"></param>
        /// <param name="bulkTagModalDialogForm"></param>
        /// <returns></returns>
        public static string CreateTagUrlHtml(string gridName, BulkTagModalDialogForm bulkTagModalDialogForm)
        {
            if (bulkTagModalDialogForm == null)
                return string.Empty;

            var tagIconHtml =
                $"<span style=\"margin-right:5px\">{BootstrapHtmlHelpers.MakeGlyphIcon("glyphicon-tag")}</span>";

            var getProjectIDFunctionString =
                $"function() {{ return {gridName}GetValuesFromCheckedGridRows('{bulkTagModalDialogForm.ValueColumnName}', '{bulkTagModalDialogForm.ReturnListName}'); }}";

            return
                ModalDialogFormHelper.ModalDialogFormLink($"{tagIconHtml}{bulkTagModalDialogForm.DialogLinkText}",
                    bulkTagModalDialogForm.DialogUrl,
                    bulkTagModalDialogForm.DialogTitle,
                    ModalDialogFormHelper.DefaultDialogWidth,
                    "Save",
                    "Cancel",
                    new List<string>(),
                    null,
                    getProjectIDFunctionString).ToString();
        }

        public static string CreateArbitraryHtml(IEnumerable<string> arbitraryHtml, string indent)
        {
            return arbitraryHtml != null ? String.Join("\r\n", arbitraryHtml.Select(x => $"{indent}{x}")) : String.Empty;
        }

        /// <summary>
        /// For making an edit icon on the grid that goes to a new page
        /// If insufficient permissions, returns empty string
        /// </summary>
        /// <param name="editUrl"></param>
        /// <param name="hasPermission"></param>
        /// <returns></returns>
        public static HtmlString MakeEditIconAsHyperlinkBootstrap(string editUrl, bool hasPermission)
        {
            return hasPermission ? UrlTemplate.MakeHrefStringWithHtmlLinkText(editUrl,
                $"{EditIconBootstrap}<span style=\"display:none\">Edit</span>") : new HtmlString(string.Empty);
        }



        /// <summary>
        /// For making an edit icon on the grid with an editor in a jquery ui dialog
        /// If insufficient permissions, returns empty string
        /// </summary>
        public static HtmlString MakeModalDialogLink(string linkHtml, string dialogContentUrl, int dialogWidth, string dialogTitle, bool hasPermission, string saveButtonText, string cancelButtonText, List<string> extraCssClasses, string onJavascriptReadyFunction, string postData)
        {
            if (hasPermission)
            {
                return ModalDialogFormHelper.ModalDialogFormLink(linkHtml, dialogContentUrl, dialogTitle, dialogWidth, saveButtonText, cancelButtonText, extraCssClasses, onJavascriptReadyFunction, postData);
            }
            return new HtmlString(String.Empty);
        }

        /// <summary>
        /// For making an edit icon on the grid with an editor in a jquery ui dialog
        /// </summary>
        public static HtmlString MakeModalDialogLink(string linkHtml, string dialogContentUrl, int dialogWidth, string dialogTitle, string onJavascriptReadyFunction)
        {
            return MakeModalDialogLink(linkHtml, dialogContentUrl, dialogWidth, dialogTitle, true, "Save", "Cancel", new List<string>(), onJavascriptReadyFunction, null);
        }



        /// <summary>
        /// For making an edit icon on the grid with an editor in a jquery ui dialog
        /// </summary>
        public static HtmlString MakeEditIconAsModalDialogLinkBootstrap(ModalDialogForm modalDialogForm)
        {
            return MakeModalDialogLink($"{EditIconBootstrap}<span style=\"display:none\">Edit</span>", modalDialogForm.ContentUrl, modalDialogForm.DialogWidth, modalDialogForm.DialogTitle, modalDialogForm.OnJavascriptReadyFunction);
        }

        /// <summary>
        /// For making an edit icon on the grid with an editor in a jquery ui dialog
        /// </summary>
        public static HtmlString MakeEditIconAsModalDialogLinkBootstrap(string editDialogUrl, string formTitle)
        {
            return MakeModalDialogLink($"{EditIconBootstrap}<span style=\"display:none\">Edit</span>", editDialogUrl, ModalDialogFormHelper.DefaultDialogWidth, formTitle, null);
        }

        /// <summary>
        /// For making an edit icon on the grid with an edit jquery ui dialog confirm.
        /// Will make a grey edit icon if edit is not possible. *** WILL NOT PREVENT AN EDIT FROM HAPPENING! YOU MUST ENFORCE AT THE CONTROLLER LEVEL ***
        /// </summary>
        /// <param name="editDialogUrl"></param>
        /// <param name="formTitle"></param>
        /// <param name="editPossibleForObject">Is an edit possible for the given object?</param>
        /// <returns></returns>
        public static HtmlString MakeEditIconAsModalDialogLinkBootstrap(string editDialogUrl, string formTitle, bool editPossibleForObject)
        {
            var editIcon = editPossibleForObject ? $"{EditIconBootstrap}<span style=\"display:none\">Edit</span>"
                : BootstrapHtmlHelpers.MakeGlyphIcon("glyphicon-edit gi-1x disabled").ToString();
            return MakeModalDialogLink(editIcon, editDialogUrl, ModalDialogFormHelper.DefaultDialogWidth, formTitle, null);
        }

        /// <summary>
        /// For making a plus icon on the grid with an editor in a jquery ui dialog
        /// </summary>
        public static HtmlString MakePlusIconAsModalDialogLinkBootstrap(string editDialogUrl, string formTitle)
        {
            return MakeModalDialogLink($"{PlusIconBootstrap}", editDialogUrl, ModalDialogFormHelper.DefaultDialogWidth, formTitle, null);
        }

        /// <summary>
        /// For making an edit icon on the grid with an editor in a jquery ui dialog
        /// </summary>
        public static HtmlString MakeLtInfoEditIconAsModalDialogLinkBootstrap(ModalDialogForm modalDialogForm)
        {
            string linkText = $"{EditIconBootstrap}<span style=\"display:none\">Edit</span>";
            List<string> extraCssClasses = new List<string>();
            return ModalDialogFormHelper.ModalDialogFormLink(null,
                linkText,
                modalDialogForm.ContentUrl,
                modalDialogForm.DialogTitle,
                modalDialogForm.DialogWidth,
                ModalDialogFormHelper.SaveButtonID,
                "Save",
                "Cancel",
                extraCssClasses,
                modalDialogForm.OnJavascriptReadyFunction,
                null,
                null);
        }



        /// <summary>
        /// For making a delete icon on the grid with a delete jquery ui dialog confirm
        /// </summary>
        /// <param name="deleteDialogUrl"></param>
        /// <param name="userHasDeletePermission"></param>
        /// <returns></returns>
        public static HtmlString MakeDeleteIconAndLinkBootstrap(string deleteDialogUrl, bool userHasDeletePermission)
        {
            return MakeDeleteIconAndLinkBootstrap(deleteDialogUrl, userHasDeletePermission, true);
        }

        /// <summary>
        /// For making a delete icon on the grid with a delete jquery ui dialog confirm.
        /// Will make a grey trash can icon if delete is not possible. *** WILL NOT PREVENT A DELETE FROM HAPPENING! YOU MUST ENFORCE AT THE CONTROLLER LEVEL ***
        /// </summary>
        /// <param name="deleteDialogUrl"></param>
        /// <param name="userHasDeletePermission">Does the given user have permission to perform a delete?</param>
        /// <param name="deletePossibleForObject">Is a delete possible for the given object?</param>
        /// <returns></returns>
        public static HtmlString MakeDeleteIconAndLinkBootstrap(string deleteDialogUrl, bool userHasDeletePermission, bool deletePossibleForObject)
        {
            var deleteIcon = deletePossibleForObject ? $"{DeleteIconBootstrap}<span style=\"display:none\">Delete</span>"
                : BootstrapHtmlHelpers.MakeGlyphIcon("glyphicon-trash gi-1x disabled").ToString();
            return ModalDialogFormHelper.MakeDeleteLink(deleteIcon, deleteDialogUrl, new List<string>(), userHasDeletePermission);
        }


        /// <summary>
        /// Typically, we either have a create new button that goes to a new page
        /// or a modal dialog version of the create new record
        /// </summary>
        /// <param name="createUrl"></param>
        /// <param name="createUrlClass"></param>
        /// <param name="createPopupForm"></param>
        /// <param name="createActionPhrase"></param>
        /// <param name="objectNameSingular"></param>
        /// <returns></returns>
        public static string CreateCreateUrlHtml(string createUrl, string createUrlClass, ModalDialogForm createPopupForm, string createActionPhrase, string objectNameSingular)
        {
            // objectNameSingular can contain user-entered data (e.g. a person's name), and createString is emitted
            // below both as element text and as a title attribute, so encode it here rather than at each use.
            var createString = HttpUtility.HtmlEncode(!string.IsNullOrEmpty(createActionPhrase) ? createActionPhrase : $"Create New {objectNameSingular}");
            var createUrlHtml = String.Empty;
            if (!String.IsNullOrWhiteSpace(createUrl))
            {
                createUrlHtml = String.Format(@"<a class=""process create {0}"" href=""{1}"" title=""{2}"">{2}</a>",
                    String.IsNullOrEmpty(createUrlClass) ? String.Empty : createUrlClass,
                    createUrl,
                    createString);
            }
            else if (createPopupForm != null)
            {
                createUrlHtml = MakeModalDialogLink($"{PlusIconBootstrap} {createString}",
                    createPopupForm.ContentUrl,
                    createPopupForm.DialogWidth,
                    createPopupForm.DialogTitle,
                    true,
                    createPopupForm.SaveButtonText,
                    createPopupForm.CancelButtonText,
                    null,
                    createPopupForm.OnJavascriptReadyFunction,
                    null).ToString();
            }
            return createUrlHtml;
        }


        /// <summary>
        /// Creates the html for the reset, load, and save grid settings buttons
        /// JS used by the HTML in this function is here: C:\git\sitkatech\projectfirma\Source\ProjectFirma.Web\ScriptsCustom\agGrid\esa-ag-grid.js
        /// </summary>
        /// <returns></returns>
        public static string CreateGridSettingsButtonsHtml(string gridName, bool showGridSettingsButtons)
        {
            const string gridSettingsButtonTemplate = @"
            <div class=""row gridSettingsButtonContainer"">
                <div id=""{0}GridSettingsLoadedError"" style=""display:none;"">
                    <div class=""col-md-12 alert alert-dismissible alert-danger""><button type=""button"" class=""close"" data-dismiss=""alert"">×</button><strong>Oh no!</strong> We did not find any saved Grid Settings to apply.</div>
                </div>
                <div id=""{0}GridSettingsSavedError"" style=""display:none;"">
                    <div class=""col-md-12 alert alert-dismissible alert-danger""><button type=""button"" class=""close"" data-dismiss=""alert"">×</button><strong>Oh no!</strong> There was an error while trying to save your Grid Settings.</div>
                </div>
                <div id=""{0}GridSettingsSavedMessage"" style=""display:none;"">
                    <div class=""col-md-12 alert alert-dismissible alert-success""><button type=""button"" class=""close"" data-dismiss=""alert"">×</button><strong>Success!</strong> Your Grid Settings were saved. </div>
                </div>
                <div id=""{0}GridSettingsMessageContainer""></div>
                <div class=""col-md-12 text-right""><button class=""btn btn-firma btn-sm"" onclick=""resetGridState({0}GridOptionsApi)"">Reset Grid</button>&nbsp;<button class=""btn btn-firma btn-sm"" onclick=""loadGridState({0}GridOptionsApi, '{0}', true)"">Load Grid Settings</button>&nbsp;<button class=""btn btn-firma btn-sm"" onclick=""saveGridState({0}GridOptionsApi, '{0}')"">Save Grid Settings</button></div>
            </div>";
            var gridSettingsButtonsHtml = String.Empty;

            if (showGridSettingsButtons)
            {
                gridSettingsButtonsHtml = string.Format(gridSettingsButtonTemplate, gridName);
            }
            
            return gridSettingsButtonsHtml;
        }
    }
}
