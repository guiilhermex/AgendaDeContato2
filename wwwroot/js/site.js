function mostrarToast(mensagem, tipo = "success") {

    const toastEl = document.getElementById("toastMensagem");

    if (!toastEl) {
        console.error("Toast não encontrado no DOM.");
        return;
    }

    toastEl.className = `toast align-items-center text-bg-${tipo} border-0`;

    const texto = document.getElementById("toastTexto");
    if (texto) texto.innerText = mensagem;

    const toast = new bootstrap.Toast(toastEl, {
        delay: 3000
    });

    toast.show();
}

document.addEventListener("DOMContentLoaded", () => {

    const nome = localStorage.getItem("nomeUsuario");
    if (nome) {
        const span = document.getElementById("nomeUsuario");
        if (span) span.innerText = nome;
    }

    const btnLogout = document.getElementById("btnLogout");
    if (btnLogout) {
        btnLogout.addEventListener("click", () => {
            localStorage.removeItem("token");
            localStorage.removeItem("nomeUsuario");
            window.location.href = "/Usuario/Login";
        });
    }
});
