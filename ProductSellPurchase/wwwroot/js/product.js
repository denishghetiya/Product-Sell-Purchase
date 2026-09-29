$(document).ready(function () {

    $("#productList").DataTable({
            searching: false,
            order: [],
            processing: true,
            serverSide: true,
            autoWidth: false,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            ajax: {
                url: "/Product/ProductList",
                type: "POST",
                contentType: "application/json",
                data: function (d) {
                    return JSON.stringify({
                        draw: d.draw,
                        start: d.start,
                        length: d.length,
                        //search: null,
                        columns: d.columns.map((col, index) => ({
                            field: col.data,
                            isSortable: col.orderable,
                            sort: d.order.find(o => o.column === index)
                                ? { direction: d.order.find(o => o.column === index).dir === "asc" ? 0 : 1 }
                                : null
                        })),
                        //filters: {
                        //    field1: $("#searchName").val(),
                        //    field2: $("#searchPhone").val(),
                        //    field3: $("#searchEmail").val(),
                        //    field4: $("#searchProject").val() 
                        //}
                    });
                }
            },
            columns: [
                { title: "Product Id", data: "productId", visible: false, searchable: false },
                { title: "Product Name", data: "productName", searchable: false, orderable: false },
                { title: "Price", data: "price" , searchable: false, orderable: false },
                { title: "CreatedBy", data: "createdBy" , searchable: false, orderable: false },
                { title: "Created Date", data: "createdDate" , searchable: false, orderable: false },
                { title: "ModifyBy", data: "modifyBy" , searchable: false, orderable: false },
                { title: "ModifyDate", data: "modifyDate" , searchable: false, orderable: false },
                {
                    title: "Action",
                    data: "productId",
                    orderable: false,
                    searchable: false,
                    width: "10%",
                    render: function (data) {
                        return `
                            <div class="btn-group" role="group">
                                <button type="button" class="btn btn-sm btn-primary dropdown-toggle" data-bs-toggle="dropdown">Action</button>
                                <ul class="dropdown-menu">
                                    <li><a class="dropdown-item btnedituser text-primary" href="/AllProduct/CreateProduct?productId=${data}" ><i class="fa fa-edit"></i>&nbsp;Edit</a></li>
                                    <li><a class="dropdown-item btndeleteuser text-danger" style="cursor: pointer;" onclick="deleteProduct(${data})" ><i class="fa fa-trash"></i>&nbsp;Delete</a></li>
                                </ul>
                            </div>`;
                    }
                }
            ]
    });

    $("#createProductForm").validate({
        rules: {
            ProductName: { required: true},
            Price: { required: true}
        },
        messages: {
            ProductName: {
                required: "ProductName is Required."
            },
            Price: {
                required: "Price is Required."
            }
        },
        errorElement: "div",
        errorClass: "text-danger small mt-1",
        highlight: function (element) {
            $(element).addClass("is-invalid");
        },
        unhighlight: function (element) {
            $(element).removeClass("is-invalid");
        },
        submitHandler: function (form) {
            $('#submitBtnCreateProduct').prop('disabled',true);
            var formData = $(form);
            $.ajax({
                url: "/Product/CreateProduct",
                type: "POST",
                data: formData.serialize(),
                success: function (response) {
                    $('#submitBtnCreateProduct').prop('disabled',false);
                    if(response.success == true){
                    Swal.fire({
                        icon: "success",
                        text: response.message
                          }).then((result) => {
                              if (result.isConfirmed) {
                                window.location.href = "/Product/ProductList";
                              }
                          });
                    }
                    if(response.success == false){
                        Swal.fire({
                          icon: "error",
                          text: response.message
                        });
                    }
                },
                error: function (res) {
                    Swal.fire({
                        icon: "error",
                        text: "Something went wrong: " + res.statusText
                    });
                }
            });
        }
    });

});
                
function deleteProduct(productId) {
    Swal.fire({
      text: "Are you sure want to delete Product ?",
      icon: "question",
      showCancelButton: true,
    }).then((result) => {
        if (result.isConfirmed) {
          window.location.href = "/Product/DeleteProduct?productId="+productId; 
        }
    });
}
