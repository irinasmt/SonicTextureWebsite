$(".s-headers").on("click",
    function () {
        var index = $(this).index();
        $(".s-headers").find(".u-dot-line-v1__inner").removeClass("active");
        $(this).find(".u-dot-line-v1__inner").addClass("active");
        $(".s-steps").hide();
        $(".s-steps").eq(index).show();
    });

$("#addTestEmail").on("click",
    function () {
        var firstEmail = $("#FirstEmail").val();
        var secondEmail = $("#SecondEmail").val();
        $.ajax({
            url: "/Home/AddTestEmail",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ "firstEmail": firstEmail, "secondEmail": secondEmail }),
            success: function (data) {
                console.log(data.length);
                if (data.length > 10000) {
                    window.location.replace("/Account/Login");
                    return;
                }
                if (data !== "True") {
                    $(".s-testEmail").find(".s-title").text("Emails were not saved!");
                    $(".s-emailMessage")
                        .text(
                            "Something went wrong. Please make sure that the emails that you have entered are corect and try again.");
                } else {
                    $(".s-testEmail").find(".s-title").text("Emails were saved!");
                    $(".s-emailMessage")
                        .text(
                        " Great, you can now to the next step where you can actually send messages to the users that you want to test with.");

                }
                $(".s-testEmail").removeClass("hide");
            },
            error: function (data) {
                console.log(data);
                alert("ups something went wrong");
            }

        });
    });

$("#testMessageApi").on("click",
    function () {
        var Email = $("#TestEmail").val();
        var message = $("#Message").val();
        $.ajax({
            url: "/testmessageapi",
            type: "POST",
            contentType: "application/json; charset=utf-8", 
            data: JSON.stringify({ "Email": Email, "Message": message }),
            success: function (data) {
                if (data.length > 10000) {
                    window.location.replace("/Account/Login");
                    return;
                }
                if (data[0].HasMessageBeenSent) {
                    $(".s-sendEmailMessage").find(".s-title")
                        .text("Message has been sent. Chek To-Do list from your Alexa App!");
                } else {
                    $(".s-sendEmailMessage").find(".s-title")
                        .text(data[0].FailureReason);
                }
                $(".s-sendEmailMessage").removeClass("hide");
            },
            error: function (data) {
                alert("ups something went wrong");
            }

        });
    }
);

$("#submitForCertification").on("click",
    function () {
        var name = $("#CompanyName").val();
        var email = $("#CompanyEmail").val();
        var intent = $("#UseCase").val();
        var example = $("#MessageSample").val();
        $.ajax({
            url: "/Home/InsertCertification",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ "name": name, "email": email, "intention": intent, "exampleMessage":example }),
            success: function (data) {
                if (data.length > 10000) {
                    window.location.replace("/Account/Login");
                    return;
                }
                if (data !== "True") {
                    $(".s-sendCertification").find(".s-message").hide();
                    $(".s-sendCertification").find(".s-title")
                        .text(
                            "Something went wrong. Make sure that all the fields are completed and that the email address is correct.");
                } else {
                    $(".s-sendCertification").find(".s-title")
                        .text("Thank you");
                    $(".s-sendCertification").find(".s-message").show();
                }
                
                $(".s-sendCertification").removeClass("hide");
            }
        });
    }
);