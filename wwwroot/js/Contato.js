const CONTATO_API = "/Contato";

let contatoExcluirId = null;
let modalContato = null;
let modalConfirmacaoContato = null;

// cache em memória
let contatosCache = [];

/* ===============================
   AUTH
================================ */
function getAuthHeaders() {
    const token = localStorage.getItem("token");

    if (!token) {
        window.location.href = "/Usuario/Login";
        return {};
    }

    return {
        "Authorization": `Bearer ${token}`,
        "Content-Type": "application/json"
    };
}

/* ===============================
   TOAST
================================ */
function mostrarToast(mensagem, tipo = "success") {
    const toastEl = document.getElementById("toastMensagem");
    const toastBody = document.getElementById("toastBody");

    if (!toastEl || !toastBody) return;

    toastEl.className = `toast align-items-center text-bg-${tipo} border-0`;
    toastBody.innerText = mensagem;

    new bootstrap.Toast(toastEl).show();
}

/* ===============================
   RENDER TABELA
================================ */
function renderizarTabela(contatos) {
    const tbody = document.getElementById("dadosTabela");
    tbody.innerHTML = "";

    contatos.forEach(contato => {
        const grupos = Array.isArray(contato.grupos) && contato.grupos.length
            ? contato.grupos.join(", ")
            : "-";

        tbody.innerHTML += `
            <tr>
                <td>${contato.nomeContato}</td>
                <td>${contato.email}</td>
                <td>${contato.telefone ?? "-"}</td>
                <td>${grupos}</td>
                <td>
                    <button class="btn btn-sm btn-warning me-1"
                        onclick="editarContato(${contato.idContato})">
                        Editar
                    </button>
                    <button class="btn btn-sm btn-danger"
                        onclick="confirmarExcluirContato(${contato.idContato})">
                        Excluir
                    </button>
                </td>
            </tr>
        `;
    });
}

/* ===============================
   LISTAR CONTATOS
================================ */
async function carregarContatos() {
    try {
        const response = await fetch(CONTATO_API, {
            headers: getAuthHeaders()
        });

        if (!response.ok) {
            mostrarToast("Erro ao carregar contatos", "danger");
            return;
        }

        const result = await response.json();
        contatosCache = result.data ?? [];

        aplicarFiltro(); // sempre respeita o filtro atual

    } catch (error) {
        console.error(error);
        mostrarToast("Erro inesperado", "danger");
    }
}

/* ===============================
   FILTRO POR NOME (AJUSTADO)
================================ */
function aplicarFiltro() {
    const input = document.getElementById("FiltroContatoNome");
    const filtro = input ? input.value.toLowerCase().trim() : "";

    if (!filtro) {
        renderizarTabela(contatosCache);
        return;
    }

    const filtrados = contatosCache.filter(c =>
        c.nomeContato.toLowerCase().includes(filtro)
    );

    renderizarTabela(filtrados);
}

/* ===============================
   EDITAR CONTATO
================================ */
async function editarContato(id) {
    try {
        const response = await fetch(`${CONTATO_API}/${id}`, {
            headers: getAuthHeaders()
        });

        if (!response.ok) {
            mostrarToast("Contato não encontrado", "danger");
            return;
        }

        const contato = (await response.json()).data;

        document.getElementById("ContatoId").value = contato.idContato;
        document.getElementById("ContatoNome").value = contato.nomeContato;
        document.getElementById("ContatoEmail").value = contato.email;
        document.getElementById("ContatoTelefone").value = contato.telefone ?? "";

        document.getElementById("ContatoGrupo").value =
            Array.isArray(contato.grupos)
                ? contato.grupos.join(", ")
                : "";

        modalContato.show();

    } catch (error) {
        console.error(error);
        mostrarToast("Erro ao carregar contato", "danger");
    }
}

/* ===============================
   SALVAR CONTATO
================================ */
async function salvarContato() {
    const id = document.getElementById("ContatoId").value;

    const nome = document.getElementById("ContatoNome").value.trim();
    const email = document.getElementById("ContatoEmail").value.trim();
    const telefone = document.getElementById("ContatoTelefone").value.trim();
    const gruposTexto = document.getElementById("ContatoGrupo").value;

    if (!nome || !email) {
        mostrarToast("Preencha os campos obrigatórios", "warning");
        return;
    }

    const payload = {
        NomeContato: nome,
        Email: email,
        Telefone: telefone,
        Grupos: gruposTexto
            ? gruposTexto.split(",").map(g => g.trim()).filter(Boolean)
            : []
    };

    const url = id
        ? `${CONTATO_API}/Editar/${id}`
        : `${CONTATO_API}/Criar`;

    const method = id ? "PUT" : "POST";

    const response = await fetch(url, {
        method,
        headers: getAuthHeaders(),
        body: JSON.stringify(payload)
    });

    if (!response.ok) {
        mostrarToast("Erro ao salvar contato", "danger");
        return;
    }

    modalContato.hide();
    document.getElementById("formContato").reset();
    document.getElementById("ContatoId").value = "";

    mostrarToast("Contato salvo com sucesso!");
    carregarContatos(); // 🔥 atualização automática
}

/* ===============================
   EXCLUIR CONTATO
================================ */
function confirmarExcluirContato(id) {
    contatoExcluirId = id;
    modalConfirmacaoContato.show();
}

async function excluirContato() {
    if (!contatoExcluirId) return;

    const response = await fetch(
        `${CONTATO_API}/Deletar/${contatoExcluirId}`,
        {
            method: "DELETE",
            headers: getAuthHeaders()
        }
    );

    if (!response.ok) {
        mostrarToast("Erro ao excluir contato", "danger");
        return;
    }

    modalConfirmacaoContato.hide();
    contatoExcluirId = null;

    mostrarToast("Contato excluído com sucesso!");
    carregarContatos(); // 🔥 atualização automática
}

/* ===============================
   INIT
================================ */
document.addEventListener("DOMContentLoaded", () => {

    modalContato = new bootstrap.Modal(
        document.getElementById("modalContato")
    );

    modalConfirmacaoContato = new bootstrap.Modal(
        document.getElementById("modalConfirmacao")
    );

    document
        .getElementById("btnSalvarContato")
        .addEventListener("click", salvarContato);

    document
        .getElementById("btnConfirmarExclusao")
        .addEventListener("click", excluirContato);

    document
        .getElementById("btnFiltrarContato")
        ?.addEventListener("click", aplicarFiltro);

    document
        .getElementById("FiltroContatoNome")
        ?.addEventListener("input", aplicarFiltro);

    carregarContatos();
});
