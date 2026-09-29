$(document).ready(function () {

    $("#userList").DataTable({
            searching: false,
            order: [],
            processing: true,
            serverSide: true,
            autoWidth: false,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            ajax: {
                url: "/User/UserList",
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
                    });
                }
            },
            columns: [
                { title: "UserId", data: "userId", visible: false, searchable: false },
                { title: "UserTypeId", data: "userTypeId", visible: false, searchable: false },
                { title: "Username", data: "username" , searchable: false, orderable: false },
                { title: "UserType Name", data: "userTypeName", searchable: false, orderable: false },
                { title: "IsActive", data: "isActive" , searchable: false, orderable: false, width: "10%",
                    render: function (data,a,othercoldata) {
                        var html = '<div class="form-check form-switch">'+
                                    `<input class="form-check-input isActiveUserStatus" type="checkbox" data-id="${othercoldata.userId}"`;
                                    if(data == true)
                                    {
                                        html += 'checked';
                                    }
                                    html += '></div>';
                        return html;
                    }
                },
                {
                    title: "Action",
                    data: "userId",
                    orderable: false,
                    searchable: false,
                    width: "10%",
                    render: function (data) {
                        return `
                            <div class="btn-group" role="group">
                                <button type="button" class="btn btn-sm btn-primary dropdown-toggle" data-bs-toggle="dropdown">Action</button>
                                <ul class="dropdown-menu">
                                    <li><a class="dropdown-item btnedituser text-primary" href="/User/EditUser?userId=${data}" ><i class="fa fa-edit"></i>&nbsp;Edit</a></li>
                                    <li><a class="dropdown-item btndeleteuser text-danger" style="cursor: pointer;" onclick="deleteUser(${data})" ><i class="fa fa-trash"></i>&nbsp;Delete</a></li>
                                </ul>
                            </div>`;
                    }
                }
            ]
    });
    $(document).on('change', '.isActiveUserStatus', function() {
        var userId = $(this).data('id');
        var isActive = $(this).is(':checked');
        $.ajax({
        url: `/User/ChangeUserStatus?userId=${userId}&isActive=${isActive}`,
        type: "POST",
        success: function (data) {
            if(data.success != true){
                Swal.fire({
                  icon: "error",
                  text: "status not changed !!!"
                });
            }
        }
        });
    });
    $("#createUserForm").validate({
        rules: {
            Username: { required: true},
            Email: { required: true},
            Password: { required: true},
            ConfirmPassword: { required: true},
            UserTypeId: { required: true},
            Image: { required: true}
        },
        messages: {
            Username: {
                required: "Username is Required."
            },
            Email: {
                required: "Email is Required."
            },
            Password: {
                required: "Password is Required."
            },
            ConfirmPassword: {
                required: "ConfirmPassword is Required."
            },
            UserTypeId: {
                required: "UserTypeId is Required."
            },
            Image: {
                required: "Image is Required."
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
            $('#createusersubbtn').prop('disabled',true);
            var formData = new FormData(form);
            $.ajax({
                url: "/User/CreateUser",
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    $('#createusersubbtn').prop('disabled',false);
                    if(response.success == true){
                    Swal.fire({
                        icon: "success",
                        text: response.message
                          }).then((result) => {
                              if (result.isConfirmed) {
                                window.location.href = "/User/UserList";
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
    $("#editUserForm").validate({
        rules: {
            Username: { required: true},
            Email: { required: true},
            Password: { required: true},
            ConfirmPassword: { required: true},
            UserTypeId: { required: true}
        },
        messages: {
            Username: {
                required: "Username is Required."
            },
            Email: {
                required: "Email is Required."
            },
            Password: {
                required: "Password is Required."
            },
            ConfirmPassword: {
                required: "ConfirmPassword is Required."
            },
            UserTypeId: {
                required: "UserTypeId is Required."
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
            $('#editusersubbtn').prop('disabled',true);
            var formData = new FormData(form);
            $.ajax({
                url: "/User/EditUser",
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    $('#editusersubbtn').prop('disabled',false);
                    if(response.success == true){
                    Swal.fire({
                        icon: "success",
                        text: response.message
                          }).then((result) => {
                              if (result.isConfirmed) {
                                window.location.href = "/User/UserList";
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


    $("#userTypeList").DataTable({
            searching: false,
            order: [],
            processing: true,
            serverSide: true,
            autoWidth: false,
            lengthMenu: [[10, 25, 50, 100], [10, 25, 50, 100]],
            ajax: {
                url: "/UserType/UserTypeList",
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
                    });
                }
            },
            columns: [
                { title: "UserTypeId", data: "userTypeId", visible: false, searchable: false },
                { title: "UserType Name", data: "userTypeName", searchable: false, orderable: false },
                { title: "IsActive", data: "isActive" , searchable: false, orderable: false, width: "10%",
                    render: function (data,a,othercoldata) {
                        var html = '<div class="form-check form-switch">'+
                                    `<input class="form-check-input isActiveTypeStatus" type="checkbox" data-id="${othercoldata.userTypeId}"`;
                                    if(data == true)
                                    {
                                        html += 'checked';
                                    }
                                    html += '></div>';
                        return html;
                    }
                },
                {
                    title: "Action",
                    data: "userTypeId",
                    orderable: false,
                    searchable: false,
                    width: "10%",
                    render: function (data) {
                        return `
                            <div class="btn-group" role="group">
                                <button type="button" class="btn btn-sm btn-primary dropdown-toggle" data-bs-toggle="dropdown">Action</button>
                                <ul class="dropdown-menu">
                                    <li><a class="dropdown-item btnedituser text-primary" href="/UserType/EditType?userTypeId=${data}" ><i class="fa fa-edit"></i>&nbsp;Edit</a></li>
                                    <li><a class="dropdown-item btndeleteuser text-danger" style="cursor: pointer;" onclick="deleteUserType(${data})" ><i class="fa fa-trash"></i>&nbsp;Delete</a></li>
                                </ul>
                            </div>`;
                    }
                }
            ]
    });
    $(document).on('change', '.isActiveTypeStatus', function() {
        var userTypeId = $(this).data('id');
        var isActive = $(this).is(':checked');
        $.ajax({
        url: `/UserType/ChangeUserTypeStatus?userTypeId=${userTypeId}&isActive=${isActive}`,
        type: "POST",
        success: function (data) {
            if(data.success != true){
                Swal.fire({
                  icon: "error",
                  text: "status not changed !!!"
                });
            }
        }
        });
    });
    $("#createTypeForm").validate({
        rules: {
            UserTypeName: { required: true},
        },
        messages: {
            UserTypeName: {
                required: "UserTypeName is Required."
            },
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
            $('#createTypeFormSubBtn').prop('disabled',true);
            //var formData = $(form);
            var formData = new FormData(form);
            $.ajax({
                url: "/UserType/CreateType",
                type: "POST",
                //data: formData.serialize(),
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    $('#createTypeFormSubBtn').prop('disabled',false);
                    if(response.success == true){
                    Swal.fire({
                        icon: "success",
                        text: response.message
                          }).then((result) => {
                              if (result.isConfirmed) {
                                window.location.href = "/UserType/UserTypeList";
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
    $("#editTypeForm").validate({
        rules: {
            UserTypeName: { required: true},
        },
        messages: {
            UserTypeName: {
                required: "UserTypeName is Required."
            },
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
            $('#editTypeFormSubBtn').prop('disabled',true);
            //var formData = $(form);
            var formData = new FormData(form);
            $.ajax({
                url: "/UserType/EditType",
                type: "POST",
                //data: formData.serialize(),
                data: formData,
                processData: false,
                contentType: false,
                success: function (response) {
                    $('#editTypeFormSubBtn').prop('disabled',false);
                    if(response.success == true){
                    Swal.fire({
                        icon: "success",
                        text: response.message
                          }).then((result) => {
                              if (result.isConfirmed) {
                                window.location.href = "/UserType/UserTypeList";
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

    $.ajax({
        url: '/SidebarMenu/GetSidebarPages',
        method: 'GET',
        success: function (pages) {
            let html = '<ul class="nav flex-column bg-white p-3 rounded shadow-sm">';
                html += '<li class="nav-item mb-3"><h5 class="text-primary fw-bold text-center">Menu</h5><hr/></li>';

            pages.forEach(function (page) {
                html += `<li class="nav-item">
                            <a class="nav-link text-dark" href="/${page.controller}/${page.action}?PageId=${page.pageId}">
                               <i class="bi bi-arrow-right-short me-2"></i> ${page.pageName}
                            </a>
                         </li>`;
            });

            html += '</ul>';
            $('#sidebarMenu').html(html);
        },
        error: function () {
            console.error("Failed to load admin sidebar.");
        }
    });

});

function UserList() {
    window.location.href = '/User/UserList';
}

function UserTypeList() {
    window.location.href = '/UserType/UserTypeList';
}

function deleteUser(userId) {
    Swal.fire({
      text: "Are you sure want to delete User ?",
      icon: "question",
      showCancelButton: true,
    }).then((result) => {
        if (result.isConfirmed) {
          $.ajax({
            url: `/User/DeleteUser?userId=${userId}`,
            type: "POST",
            success: function (response) {
                if(response.success == true){
                    Swal.fire({
                        icon: "success",
                        text: response.message
                          }).then((result) => {
                              if (result.isConfirmed) {
                                window.location.reload();
                              }
                          });
                }
                if(response.success == false){
                    Swal.fire({
                      icon: "error",
                      text: response.message
                    });
                }
            }
          });
        }
    });
}
function deleteUserType(usertypeid) {
    Swal.fire({
      text: "Are you sure want to delete UserType ?",
      icon: "question",
      showCancelButton: true,
    }).then((result) => {
        if (result.isConfirmed) {
          $.ajax({
            url: `/UserType/DeleteType?usertypeid=${usertypeid}`,
            type: "POST",
            success: function (response) {
                if(response.success == true){
                    Swal.fire({
                        icon: "success",
                              text: response.message
                          }).then((result) => {
                              if (result.isConfirmed) {
                                window.location.reload();
                              }
                          });
                }
                if(response.success == false){
                    Swal.fire({
                      icon: "error",
                      text: response.message
                    });
                }
            }
          });
        }
    });
}