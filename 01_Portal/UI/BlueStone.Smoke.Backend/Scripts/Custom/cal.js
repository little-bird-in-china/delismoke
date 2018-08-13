/** 
* 加法运算，避免数据相加小数点后产生多位数和计算精度损失。 
* @param num1加数1 | num2加数2 
*/
Math.add = function (num1, num2) {
    num1 = parseFloat(num1);
    num2 = parseFloat(num2);
    if (isNaN(num1) || isNaN(2)) {
        throw '请输入数字';
    }
    var baseNum, baseNum1, baseNum2, result;
    try {
        baseNum1 = num1.toString().split(".")[1].length;
    } catch (e) {
        baseNum1 = 0;
    }
    try {
        baseNum2 = num2.toString().split(".")[1].length;
    } catch (e) {
        baseNum2 = 0;
    }
    baseNum = Math.pow(10, Math.max(baseNum1, baseNum2));
    result = (num1 * baseNum + num2 * baseNum) / baseNum;
    return parseFloat(result);
}
/** 
* 减法运算，避免数据相减小数点后产生多位数和计算精度损失。 
* 
* @param num1被减数 | num2减数 
*/
Math.sub = function (num1, num2) {
    num1 = parseFloat(num1);
    num2 = parseFloat(num2);
    if (isNaN(num1) || isNaN(2)) {
        throw '请输入数字';
    }
    var baseNum, baseNum1, baseNum2, result;
    var precision;// 精度 
    try {
        baseNum1 = num1.toString().split(".")[1].length;
    } catch (e) {
        baseNum1 = 0;
    }
    try {
        baseNum2 = num2.toString().split(".")[1].length;
    } catch (e) {
        baseNum2 = 0;
    }
    baseNum = Math.pow(10, Math.max(baseNum1, baseNum2));
    precision = (baseNum1 >= baseNum2) ? baseNum1 : baseNum2;
    result = ((num1 * baseNum - num2 * baseNum) / baseNum).toFixed(precision);
    return parseFloat(result);
};
/** 
* 乘法运算，避免数据相乘小数点后产生多位数和计算精度损失。 
* 
* @param num1被乘数 | num2乘数 
*/
Math.mult = function (num1, num2) {
    num1 = parseFloat(num1);
    num2 = parseFloat(num2);
    if (isNaN(num1) || isNaN(2)) {
        throw '请输入数字';
    }
    var baseNum = 0, result;
    try {
        baseNum += num1.toString().split(".")[1].length;
    } catch (e) {
    }
    try {
        baseNum += num2.toString().split(".")[1].length;
    } catch (e) {
    }
    result = Number(num1.toString().replace(".", "")) * Number(num2.toString().replace(".", "")) / Math.pow(10, baseNum);
    return parseFloat(result);
};
/** 
* 除法运算，避免数据相除小数点后产生多位数和计算精度损失。 
* 
* @param num1被除数 | num2除数 
*/
Math.div = function (num1, num2) {
    num1 = parseFloat(num1);
    num2 = parseFloat(num2);
    if (isNaN(num1) || isNaN(2)) {
        throw '请输入数字';
    }
    var baseNum1 = 0, baseNum2 = 0;
    var baseNum3, baseNum4,result;
    try {
        baseNum1 = num1.toString().split(".")[1].length;
    } catch (e) {
        baseNum1 = 0;
    }
    try {
        baseNum2 = num2.toString().split(".")[1].length;
    } catch (e) {
        baseNum2 = 0;
    }
    with (Math) {
        baseNum3 = Number(num1.toString().replace(".", ""));
        baseNum4 = Number(num2.toString().replace(".", ""));
        result = (baseNum3 / baseNum4) * pow(10, baseNum2 - baseNum1);
        return parseFloat(result);
    }
};