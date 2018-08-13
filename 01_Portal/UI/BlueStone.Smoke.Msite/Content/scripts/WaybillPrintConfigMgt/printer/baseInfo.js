$(document).ready(function(){
	$('#saveBut').click(function(){
		var but = $(this);
		var baseInfo = {};
		$('.fieldInput').each(function(){
			baseInfo[$(this).attr('id')] = $(this).val();
		})
		
		$.ajaxq('saveBaseInfo',{
				url:'/printer/printerAction!baseInfo.action?isDefault=no',
				type:'POST',
				data:{baseInfo:JSON.stringify(baseInfo)},
				dataType:'text',
				cache:false,
				async:true,
				timeout:30000,
				beforeSend: function(){
					but.button('loading');
			    },
			    complete: function(){
			    	but.button('reset');
			    },
			    success: function(data){
			    	var resultJson=eval('('+data+')');
					if(resultJson.result=="error"){
						layer.alert(resultJson.msg);
					}else{
						layer.alert('保存成功');
						if(parent.appV!=null && parent.appV.baseInfoSetSuccess!=null){
							parent.appV.baseInfoSetSuccess.call(this,baseInfo);
						}
					}
				}
			});
	})
})