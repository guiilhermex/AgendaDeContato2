document.addEventListener("DOMContentLoaded", () => {
    const form = document.getElementById("cadastroForm");
    console.log(form);

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const nome = form.Nome.value.trim();
        const email = form.Email.value.trim();
        const senha = form.Senha.value.trim();

        const usuario = {
            nome: nome,
            email: email,
            senha: senha
        };

        try {
            const response = await fetch("/Usuario/Cadastro", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(usuario)
            });

            const data = await response.json();

            if (response.status === 201) {
                mostrarToast("Usuário cadastrado com sucesso!", "success");

                setTimeout(() => {
                    window.location.href = "/Usuario/Login";
                }, 2000);

                return;
            }

            if (response.status === 409) {
                mostrarToast("E-mail já cadastrado.", "warning");
                return;
            }

            if (!response.ok) {
                mostrarToast(
                    data?.errors || "Não foi possível realizar o cadastro.",
                    "danger"
                );
                return;
            }

        } catch (error) {
            console.error("Erro na requisição:", error);
            mostrarToast("Erro inesperado. Veja o console.", "danger");
        }
    });
});
