(() => {
    async function login(username, password) {
        $.ajax({
            type: "POST",
            url: "/api/authentication/login",
            data: {
                username: username,
                password: password
            },
            error: e => {
                $("p.error").html(e.responseJSON.error)
            },
            success: e => {
                document.cookie = `access-token=${e.token};expires=${new Date(3000, 0, 1).toUTCString()};path=/`;
                window.location.href = "/";
            }
        })
    }

    $("#login-button").on("click", () => {
        let username = $("#login-username").val();
        let password = $("#login-password").val();
        login(username, password)
    })
})()