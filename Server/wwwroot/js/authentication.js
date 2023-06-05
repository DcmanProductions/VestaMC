(() => {
    async function login(username, password) {
        let response = await $.post("/api/authentication/login", {
            username=username,
            password=password
        });
        if (response.error) {
            $("p.error").html(response.error)
        } else {
            document.cookie = `access-token=${response.token};expires=${new Date(3000).toUTCString()};path=/`;
        }
    }

    $("#login-button").on("click", () => {
        let username = $("#login-username").val();
        let password = $("#login-password").val();
        login(username, password)
    })
})()