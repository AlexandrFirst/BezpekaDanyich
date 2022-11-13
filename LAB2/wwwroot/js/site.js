function SendEncryptedUserInfo(key, data)
{
    const encrypt = new JSEncrypt();
    encrypt.setPublicKey(atob(key));

    const encryptedMail = encrypt.encrypt(data.mail);
    const encryptedPassword = encrypt.encrypt(data.pass);

    console.log("Encoded mail: " + encryptedMail);
    console.log("Encoded password: " + encryptedPassword);

    const dataToSend = {
        mail: encryptedMail,
        password: encryptedPassword
    }

    const modal = $("#modal");

    $.ajax({
        type: "POST",
        url: "/RSA/Login",
        data: JSON.stringify(dataToSend),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data)
        {
            $("#resultInfo").text(data.message);
            modal.modal();
        },
        error: function (errMsg)
        {
            console.log(errMsg)
            $("#resultInfo").text(JSON.parse(errMsg.responseText).message);
            modal.modal();
        }
    });
}

function SendMessageToDecrypts(data) {
    const encrypt = new JSEncrypt();
    const dataToEncrypt = JSON.stringify(data);
    var Bits = 2048;

    var MattsRSAkey = cryptico.generateRSAKey(dataToEncrypt, Bits);
    debugger;
}
