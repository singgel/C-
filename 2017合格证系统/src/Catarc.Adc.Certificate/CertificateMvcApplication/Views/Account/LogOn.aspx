<%--<link href="../../Content/bootstrap2.3.2.min.css" rel="stylesheet" type="text/css"/>
<link href="../../Content/bootstrap-responsive2.3.2.min.css" rel="stylesheet" type="text/css"/>
<link href="../../Content/font-awesome.min.css" rel="stylesheet" type="text/css"/>
<link href="../../Content/style-metro.css" rel="stylesheet" type="text/css"/>
<link href="../../Content/style.css" rel="stylesheet" type="text/css"/>
<link href="../../Content/style-responsive.css" rel="stylesheet" type="text/css"/>
<link href="../../Content/default.css" rel="stylesheet" type="text/css" id="style_color"/>
<link href="../../Content/uniform.default.css" rel="stylesheet" type="text/css"/>
<link href="../../Content/login.css" rel="stylesheet" type="text/css"/>
<script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
<script src="../../Scripts/Account/LogOn.js" type="text/javascript"></script>

<body class="login" style="background: url(../Image/certificate.png);">

	<!-- BEGIN LOGO -->

	<div class="logo">

		<!--<img src="../image/logo1.png" alt="" />-->

	</div>

	<!-- END LOGO -->

	<!-- BEGIN LOGIN -->

	<div class="content">

		<!-- BEGIN LOGIN FORM -->

		<form class="form-vertical login-form" >

			<!--<h3 class="form-title">Login</h3>-->

			<div class="alert alert-error hide">

				<button class="close" data-dismiss="alert"></button>

				<span>企业公告用户名密码不能为空.</span>

			</div>
			<div class="alert alert-nouser hide">

				<button class="close" data-dismiss="alert"></button>

				<span>用户名密码错误.</span>

			</div>
			<div class="control-group">

				<!--ie8, ie9 does not support html5 placeholder, so we just show field title for that-->

				<label class="control-label visible-ie8 visible-ie9">用户名</label>

				<div class="controls">

					<div class="input-icon left">

						<i class="icon-user"></i>

						<input class="m-wrap placeholder-no-fix" type="text" placeholder="用户名" id="username" name="username" style="height:32px;line-height:32px;"/>

					</div>

				</div>

			</div>

			<div class="control-group">

				<label class="control-label visible-ie8 visible-ie9">密码</label>

				<div class="controls">

					<div class="input-icon left">

						<i class="icon-lock"></i>

						<input class="m-wrap placeholder-no-fix" type="password" placeholder="密码" id="password" name="password" style="height:32px;line-height:32px;"/>

					</div>

				</div>

			</div>

			<div class="form-actions">

				<label class="checkbox">

			<!-- 	<input type="checkbox" name="isRememberMe" value="1"/>记住我-->
				
				</label>

				<button  class="btn blue pull-right"> <!--type="submit"-->

				<a href="javascript:login();"  >登录 <i class="m-icon-swapright m-icon-white"></i></a>

				</button>            

			</div>

		</form>
		<!-- END REGISTRATION FORM -->

	</div>

	<!-- END LOGIN -->
    
</body>--%>
<!DOCTYPE html>
<html>
<head lang="en">
    <meta charset="UTF-8">
    <title>合格证数据资源目录展示系统</title>
    <style type="text/css">
        * {
            box-sizing: border-box;
        }
        body {
            margin: 0; 
            padding: 0;
            <%--font: 16px/20px microsft yahei;--%>
        }
        .wrap {
            width: 100%;
            height: 100%;
            padding: 40px 0;
            position: fixed;
            top: 50%;
            margin-top: -382px;
            opacity: 0.6;
            background: linear-gradient(to bottom right,#5599FF,#5500DD);
            background: -webkit-linear-gradient(to bottom right,#5599FF,#5500DD);
        }
        .container {
            width: 60%;
            margin: 0 auto;
            padding-top:100px;
        }
        .container h1 {
            text-align: center;
            color: #FFFFFF;
            font-weight: 500;
        }
        .container input {
            width: 320px;
            display: block;
            height: 36px;
            border: 0;
            outline: 0;
            padding: 6px 10px;
            line-height: 24px;
            margin: 32px auto;
            -webkit-transition: all 0s ease-in 0.1ms;
            -moz-transition: all 0s ease-in 0.1ms;
            transition: all 0s ease-in 0.1ms;
        }
        .container input[type="text"] , .container input[type="password"]  {
            background-color: #FFFFFF;
            font-size: 16px;
            color: #50a3a2;
        }
        .container input[type='submit'] {
            font-size: 16px;
            letter-spacing: 2px;
            color: #666666;
            background-color: #FFFFFF;
        }
        .container input:focus {
            width: 400px;
        }
        .container input[type='submit']:hover {
            cursor: pointer;
            width: 400px;
        }

        .wrap ul {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            z-index: -10;
        }
        .wrap ul li {
            list-style-type: none;
            display: block;
            position: absolute;
            bottom: -120px;
            width: 15px;
            height: 15px;
            z-index: -8;
            background-color:rgba(255, 255, 255, 0.15);
            animotion: square 25s infinite;
            -webkit-animation: square 25s infinite;
        }
        .wrap ul li:nth-child(1) {
            left: 0;
            animation-duration: 10s;
            -moz-animation-duration: 10s;
            -o-animation-duration: 10s;
            -webkit-animation-duration: 10s;
        }
        .wrap ul li:nth-child(2) {
            width: 40px;
            height: 40px;
            left: 10%;
            animation-duration: 15s;
            -moz-animation-duration: 15s;
            -o-animation-duration: 15s;
            -webkit-animation-duration: 15s;
        }
        .wrap ul li:nth-child(3) {
            left: 20%;
            width: 25px;
            height: 25px;
            animation-duration: 12s;
            -moz-animation-duration: 12s;
            -o-animation-duration: 12s;
            -webkit-animation-duration: 12s;
        }
        .wrap ul li:nth-child(4) {
            width: 50px;
            height: 50px;
            left: 30%;
            -webkit-animation-delay: 3s;
            -moz-animation-delay: 3s;
            -o-animation-delay: 3s;
            animation-delay: 3s;
            animation-duration: 12s;
            -moz-animation-duration: 12s;
            -o-animation-duration: 12s;
            -webkit-animation-duration: 12s;
        }
        .wrap ul li:nth-child(5) {
            width: 60px;
            height: 60px;
            left: 40%;
            animation-duration: 10s;
            -moz-animation-duration: 10s;
            -o-animation-duration: 10s;
            -webkit-animation-duration: 10s;
        }
        .wrap ul li:nth-child(6) {
            width: 75px;
            height: 75px;
            left: 50%;
            -webkit-animation-delay: 7s;
            -moz-animation-delay: 7s;
            -o-animation-delay: 7s;
            animation-delay: 7s;
        }
        .wrap ul li:nth-child(7) {
            left: 60%;
            animation-duration: 8s;
            -moz-animation-duration: 8s;
            -o-animation-duration: 8s;
            -webkit-animation-duration: 8s;
        }
        .wrap ul li:nth-child(8) {
            width: 90px;
            height: 90px;
            left: 70%;
            -webkit-animation-delay: 4s;
            -moz-animation-delay: 4s;
            -o-animation-delay: 4s;
            animation-delay: 4s;
        }
        .wrap ul li:nth-child(9) {
            width: 100px;
            height: 100px;
            left: 80%;
            animation-duration: 20s;
            -moz-animation-duration: 20s;
            -o-animation-duration: 20s;
            -webkit-animation-duration: 20s;
        }
        .wrap ul li:nth-child(10) {
            width: 120px;
            height: 120px;
            left: 90%;
            -webkit-animation-delay: 6s;
            -moz-animation-delay: 6s;
            -o-animation-delay: 6s;
            animation-delay: 6s;
            animation-duration: 30s;
            -moz-animation-duration: 30s;
            -o-animation-duration: 30s;
            -webkit-animation-duration: 30s;
        }

        @keyframes square {
            0%  {
                    -webkit-transform: translateY(0);
                    transform: translateY(0)
                }
            100% {
                    bottom: 400px;
                    transform: rotate(600deg);
                    -webit-transform: rotate(600deg);
                    -webkit-transform: translateY(-500);
                    transform: translateY(-500)
            }
        }
        @-webkit-keyframes square {
            0%  {
                -webkit-transform: translateY(0);
                transform: translateY(0)
            }
            100% {
                bottom: 400px;
                transform: rotate(600deg);
                -webit-transform: rotate(600deg);
                -webkit-transform: translateY(-500);
                transform: translateY(-500)
            }
        }
    </style>
</head>
<script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
<script src="../../Scripts/Account/LogOn.js" type="text/javascript"></script>
<body>
    <div class="wrap">
        <div class="container">
            <h1>
                合格证数据资源目录展示系统</h1>
            <form>
            <input id="username" name="username" type="text" placeholder="用户名" />
            <input id="password" name="password" type="password" placeholder="密码" />
            <a href="javascript:login();">
                <input type="submit" value="账号登录" /></a>
            </form>
        </div>
        <ul>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
        </ul>
    </div>
</body>
</html>
