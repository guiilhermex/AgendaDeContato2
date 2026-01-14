document.addEventListener("DOMContentLoaded", () => {

    const form = document.getElementById("loginForm");
    if (!form) return;

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const email = form.Email.value.trim();
        const senha = form.Senha.value.trim();

        if (!email || !senha) {
            mostrarToast("Informe e-mail e senha", "warning");
            return;
        }

        try {
            const response = await fetch("/Usuario/Login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ email, senha })
            });

            if (response.status === 401) {
                mostrarToast("E-mail ou senha inválidos", "warning");
                return;
            }

            if (!response.ok) {
                mostrarToast("Não foi possível realizar o login.", "danger");
                return;
            }

            const data = await response.json();

            if (!data?.token) {
                mostrarToast("Token não recebido do servidor", "danger");
                return;
            }

            localStorage.setItem("token", data.token);
            localStorage.setItem("usuarioNome", data.nome ?? "Usuário");

            mostrarToast(`Bem-vindo, ${data.nome ?? "usuário"}!`, "success");

            setTimeout(() => {
                window.location.href = "/Dashboard/Index";
            }, 800);

        } catch (error) {
            console.error("Erro na requisição:", error);
            mostrarToast("Erro inesperado. Veja o console.", "danger");
        }
    });
});
