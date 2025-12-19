document.addEventListener("DOMContentLoaded", () => {

    const form = document.getElementById("loginForm");
    if (!form) return;

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const email = form.Email.value.trim();
        const senha = form.Senha.value.trim();

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

            if (data?.token) {
                localStorage.setItem("token", data.token);
            }

            const nome = data?.nome ?? "usuário";

            mostrarToast(`Bem-vindo, ${nome}!`, "success");

            setTimeout(() => {
                window.location.href = "/Dashboard/Index";
            }, 1000);

        } catch (error) {
            console.error("Erro na requisição:", error);
            mostrarToast("Erro inesperado. Veja o console.", "danger");
        }
    });
});
