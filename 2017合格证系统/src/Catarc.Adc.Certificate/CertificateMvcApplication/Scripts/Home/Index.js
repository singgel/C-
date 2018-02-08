$(function () {
    initZTree();
});

function initZTree() {
    var setting = {
        data: {
            simpleData: {
                enable: true
            }
        },
        callback: {
            onClick: zTreeOnClick
        }
    };
    $.ajax({
        method: 'POST',
        url: '/Home/ZTree',
        success: function (data) {
            $.fn.zTree.init($("#treeDemo"), setting, data);
        },
        error: function () {
            alert('error');
        }
    });
}
function zTreeOnClick(event, treeId, treeNode) {
    if (treeNode.getParentNode() == null) {
        return;
    }
    $.ajax({
        method: 'GET',
        url: '/Home/TData?parentName=' + treeNode.getParentNode().name + '&name=' + treeNode.name,
        success: function (data) {
            initAllData(treeNode.getParentNode().name, treeNode.name, data);
            initDownload(treeNode.getParentNode().name, treeNode.name);
        },
        error: function () {
            alert('error');
        }
    });
};
function initAllData(parentName, name, data) {
    var tbody = "";
    $.each(data, function (n, value) {
        var trs = "<tr>";
        trs += "<td nowrap='nowrap' class=\'mybtn\' ><a href=\"javascript:showData(\'" + parentName + "\',\'" + name + "\',\'" + value.H_ID + "\');\"  >查看</a></td>";
        trs += "<td>" + value.H_ID + "</td> <td>" + value.CJH + "</td>";
        trs += "<td>" + value.WZHGZBH + "</td> <td>" + value.FZRQ + "</td>";
        trs += "<td>" + value.CLZZQYMC + "</td> <td>" + value.CLLX + "</td>";
        trs += "<td>" + value.CLMC + "</td> <td>" + value.CLPP + "</td>";
        trs += "<td>" + value.CLXH + "</td> <td>" + value.DPXH + "</td>";
        trs += "<td>" + value.DPHGZBH + "</td> <td>" + value.FDJXH + "</td>";
        trs += "</tr>";
        tbody += trs;
    });
    $("#datatable").html(tbody);
}
function initDownload(parentName, name) {
    $("#btnDownload").attr("href", '/DownLoad/Download?parentName=' + parentName + '&name=' + name);
    $("#btnDownload").html('下载文件：' + name);
}
function showData(parentName, name, dataid) {
    var dataTranslate = { H_ID: "申请序列号", QYID_BJ: "QYID_BJ", QYID: "QYID", CLZTXX: "车辆状态信息", WZHGZBH: "完整合格证编号"
    , FZRQ: "发证日期", CLZZQYMC: "车辆制造企业名称", CLLX: "车辆类型", CLMC: "车辆名称", CLPP: "车辆品牌"
    , CLXH: "车辆型号", CLYS: "车辆颜色", DPXH: "底盘型号", DPID: "地盘ID", DPHGZBH: "地盘合格证编号"
    , CLSBDM: "车辆识别代号", CJH: "车架号", FDJH: "发动机号", FDJXH: "发动机型号", RLZL: "燃料种类"
    , PFBZ: "排放标准", PL: "排量", GL: "功率", ZXXS: "转向形式", QLJ: "前轮距"
    , HLJ: "后轮距", LTS: "轮胎数", LTGG: "轮胎规格", GBTHPS: "钢板弹簧片数", ZJ: "轴距"
    , ZH: "轴荷", ZS: "轴数", WKC: "外廓长", WKK: "外廓宽", WKG: "外廓高"
    , HXNBC: "货箱内部长", HXNBK: "货箱内部宽", HXNBG: "货箱内部高", ZHL: "总货量", ZZL: "ZZL"
    , EDZZL: "额定载质量", ZBZL: "整备质量", ZZLLYXS: "载质量利用系数", ZQYZZL: "准牵引总质量", EDZK: "额定载客"
    , BGCAZZDYXZZL: "半挂车鞍座最大允许总质量", JSSZCRS: "驾驶室准乘人数", QZDFS: "QZDFS", HZDFS: "HZDFS", QZDCZFS: "QZDCZFS"
    , HZDCZFS: "HZDCZFS", ZGCS: "ZGCS", ZXZS: "转向轴数", ZGSJCS: "最高设计车速", CLZZRQ: "车辆制造日期"
    , BZ: "备注", QYBZ: "企业标准", CPSCDZ: "产品生产地址", CLSCDWMC: "车辆生产单位名称", YH: "油耗"
    , CDDBJ: "纯电动标记", VERCODE: "VERCODE", HD_HOST: "HD_HOST", RESPONSE_CODE: "RESPONSE_CODE", CLIENT_HARDWARE_INFO: "CLIENT_HARDWARE_INFO"
    , APPLICMEMO: "APPLICMEMO", APPLICTYPE: "APPLICTYPE", APPLICTIME: "APPLICTIME", STATUS: "STATUS", APPROVETIME: "APPROVETIME"
    , APPROVEUSER: "APPROVEUSER", APPROVEMEMO: "APPROVEMEMO", CPH: "产品号", PC: "批次", GGSXRQ: "公告生效日期"
    , UKEY: "UKEY", VERSION: "VERSION", ZZBH: "纸张编号", DYWYM: "DYWYM", DYWYN: "打印唯一码"
    , UPSEND_TAG: "U盾标识", PZXLH: "配置序列号", QYQTXX: "企业其他信息", FIRSTGETTIME: "合格证首次上传时间", LASTGETTIME: "合格证最后上传时间"
    , CZRQ: "合格证打印时间", FEEDBACKTIME: "FEEDBACKTIME", FEEDBACKEMEMO: "FEEDBACKEMEMO", CREATETIME: "创建时间", UPDATETIME: "更新时间"
    , HD_USER: "HD_USER", ZCHGZBH: "是否免征", LSPZXLH: "临时配置序列号", IMPORTFLAG: "IMPORTFLAG", HSJE: "含税金额"
    , TypeCode: "TypeCode", InvNo: "发票编号", FPLX: "发票类型", PFLX: "发票类型", CLSBDH: "CLSBDH"
    };

    $.ajax({
        method: 'GET',
        dataType: 'json',
        url: '/Home/QueryByID?parentName=' + parentName + '&name=' + name + '&id=' + dataid,
        success: function (data) {

            var htmlContent = "";
            var i = 0;
            for (var item in data) {
                if (i == 0) {
                    htmlContent += '<tr><td>' + dataTranslate[item] + '</td><td>' + data[item] + '</td>';
                    i += 1;
                } else if (i % 3 == 0) {
                    htmlContent += '</tr><tr><td>' + dataTranslate[item] + '</td><td>' + data[item] + '</td>';
                    i += 1;
                } else {
                    htmlContent += '<td>' + dataTranslate[item] + '</td><td>' + data[item] + '</td>';
                    i += 1;
                }
            }

            layer.open({
                title: "车架号：" + data["CJH"], 
                closeBtn: 1,
                //                shadeClose: true,
                area: ['1000px', '500px'],
                content: '<table class="table table-striped">' + htmlContent + '</table>'
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            var state = XMLHttpRequest.readyState;
            alert('error');
        }
    });
}

//$('#pagination').jqPaginator({
//    totalPages: 100,
//    visiblePages: 7,
//    currentPage: 1,
//    first: '<li class="first"><a href="javascript:void(0);">首页</a></li>',
//    prev: '<li class="prev"><a href="javascript:void(0);">上一页</a></li>',
//    next: '<li class="next"><a href="javascript:void(0);">下一页</a></li>',
//    last: '<li class="last"><a href="javascript:void(0);">尾页</a></li>',
//    page: '<li class="page"><a href="javascript:void(0);">{{page}}</a></li>',
//    onPageChange: function (num) {
//        $('#totalCount').html('当前第' + num + '页');
//    }
//});