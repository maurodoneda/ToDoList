
function DeleteId(id){
  
    $.ajax({

        url: '/ToDoes/DeleteConfirmed',
        data: { id: id },

        success: function (result) {
            $('#tableDiv').html(result);
        }
        
    });

    
}

