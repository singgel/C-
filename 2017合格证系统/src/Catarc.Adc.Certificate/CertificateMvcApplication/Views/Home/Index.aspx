<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    主页
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../../Content/Home/Index.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        
       
    </style>
    <div class="row-fluid">
        <div class="con span12">
            <div class="span3 mytree">
                <ul id="treeDemo" class="ztree">
                </ul>
            </div>
            <div class="span9 mybtn">
                <a id="btnDownload" type="button" >链接按钮</a>
            </div>
            
            <div class="span9 mytable">
                <table class="table table-bordered table-striped" style="min-width:800px;overflow-y:scroll" >
                    <thead>
                        <tr>
                            <th nowrap='nowrap' style ="width:60px;">操作</th>
                            <th nowrap='nowrap'>申请序列号</th>
                            <th nowrap='nowrap'>车架号</th>
                            <th nowrap='nowrap'>完整合格证编号</th>
                            <th nowrap='nowrap'>发证日期</th>
                            <th nowrap='nowrap'>车辆制造企业名称</th>
                            <th nowrap='nowrap'>车辆类型</th>
                            <th nowrap='nowrap'>车辆名称</th>
                            <th nowrap='nowrap'>车辆品牌</th>
                            <th nowrap='nowrap'>车辆型号</th>
                            <th nowrap='nowrap'>底盘型号</th>
                            <th nowrap='nowrap'>地盘合格证编号</th>
                            <th nowrap='nowrap'>发动机型号</th>
                        </tr>
                    </thead>
                    <tbody id="datatable">
                    </tbody>
                </table>
            </div>
            <%--<div class="pagination">
            <div id="pagesize">
                <span class="totalCount" id="totalCount"></span>
            </div>
            <ul  id="pagination" style="float:right;"></ul>
        </div>--%>
        </div>
    </div>
    <script src="../../Scripts/Home/Index.js?ver=20171127-01" type="text/javascript"></script>
</asp:Content>
