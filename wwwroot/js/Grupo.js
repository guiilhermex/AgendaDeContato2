const GRUPO_API = "/Grupo";

let grupoExcluirId = null;
let modalGrupo = null;
let modalConfirmacaoGrupo = null;
let gruposCache = [];

/* ===============================
   AUTH
================================ */
function getAuthHeaders() {
    const token = localStorage.getItem("token");

    if (!token) {
        mostrarToast("Sessão expirada. Faça login novamente.", "danger");
        window.location.href = "/Usuario/Login";
        return {};
    }

    return {
        "Authorization": `Bearer ${token}`,
        "Content-Type": "application/json"
    };
}

/* ===============================
   INIT
================================ */
document.addEventListener("DOMContentLoaded", () => {

    modalGrupo = new bootstrap.Modal(
        document.getElementById("modalGrupo")
    );

    modalConfirmacaoGrupo = new bootstrap.Modal(
        document.getElementById("modalConfirmacao")
    );

    document
        .getElementById("btnSalvarGrupo")
        ?.addEventListener("click", salvarGrupo);

    document
        .getElementById("btnConfirmarExclusao")
        ?.addEventListener("click", excluirGrupo);

    // 🔥 filtro por clique
    document
        .getElementById("btnFiltrarGrupo")
        ?.addEventListener("click", aplicarFiltroGrupo);

    // 🔥 filtro enquanto digita
    document
        .getElementById("FiltroGrupoNome")
        ?.addEventListener("input", aplicarFiltroGrupo);

    carregarGrupos();
});

/* ===============================
   LISTAR GRUPOS
================================ */
async function carregarGrupos() {
    try {
        const response = await fetch(
            `${GRUPO_API}?pageNumber=1&pageSize=100`,
            { headers: getAuthHeaders() }
        );

        if (!response.ok) {
            mostrarToast("Erro ao carregar grupos", "danger");
            return;
        }

        const result = await response.json();
        gruposCache = result.data?.data ?? [];

        renderizarTabelaGrupos(gruposCache);

    } catch (error) {
        console.error(error);
        mostrarToast("Erro inesperado", "danger");
    }
}

/* ===============================
   RENDER
================================ */
function renderizarTabelaGrupos(grupos) {
    const tbody = document.getElementById("dadosTabelaGrupo");
    tbody.innerHTML = "";

    grupos.forEach(grupo => {
        tbody.innerHTML += `
            <tr>
                <td>${grupo.nomeGrupo}</td>
                <td>
                    <button class="btn btn-sm btn-warning me-1"
                        onclick="editarGrupo(${grupo.idGrupo})">
                        Editar
                    </button>
                    <button class="btn btn-sm btn-danger"
                        onclick="confirmarExcluirGrupo(${grupo.idGrupo})">
                        Excluir
                    </button>
                </td>
            </tr>
        `;
    });
}

/* ===============================
   FILTRO (CLICK + DIGITAÇÃO)
================================ */
function aplicarFiltroGrupo() {
    const filtro = document
        .getElementById("FiltroGrupoNome")
        .value
        .toLowerCase()
        .trim();

    if (!filtro) {
        renderizarTabelaGrupos(gruposCache);
        return;
    }

    const filtrados = gruposCache.filter(g =>
        g.nomeGrupo.toLowerCase().includes(filtro)
    );

    renderizarTabelaGrupos(filtrados);
}

/* ===============================
   EDITAR GRUPO
================================ */
async function editarGrupo(id) {
    try {
        const response = await fetch(`${GRUPO_API}/Listar/${id}`, {
            headers: getAuthHeaders()
        });

        if (!response.ok) {
            mostrarToast("Grupo não encontrado", "danger");
            return;
        }

        const grupo = (await response.json()).data;

        document.getElementById("GrupoId").value = grupo.idGrupo;
        document.getElementById("GrupoNome").value = grupo.nomeGrupo;
        document.getElementById("tituloModalGrupo").innerText = "Editar Grupo";

        modalGrupo.show();

    } catch (error) {
        console.error(error);
        mostrarToast("Erro ao carregar grupo", "danger");
    }
}

/* ===============================
   SALVAR (CRIAR / EDITAR)
================================ */
async function salvarGrupo() {
    const id = document.getElementById("GrupoId").value;
    const nomeGrupo = document.getElementById("GrupoNome").value.trim();

    if (!nomeGrupo) {
        mostrarToast("Informe o nome do grupo", "warning");
        return;
    }

    const payload = {
        NomeGrupo: nomeGrupo // ✅ CORREÇÃO DO 400
    };

    const url = id
        ? `${GRUPO_API}/Editar/${id}`
        : `${GRUPO_API}/Criar`;

    const method = id ? "PUT" : "POST";

    try {
        const response = await fetch(url, {
            method,
            headers: getAuthHeaders(),
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            mostrarToast("Erro ao salvar grupo", "danger");
            return;
        }

        modalGrupo.hide();
        document.getElementById("formGrupo").reset();
        document.getElementById("GrupoId").value = "";
        document.getElementById("tituloModalGrupo").innerText = "Novo Grupo";

        mostrarToast("Grupo salvo com sucesso!");
        carregarGrupos();

    } catch (error) {
        console.error(error);
        mostrarToast("Erro inesperado", "danger");
    }
}

/* ===============================
   EXCLUIR GRUPO
================================ */
function confirmarExcluirGrupo(id) {
    grupoExcluirId = id;
    modalConfirmacaoGrupo.show();
}

async function excluirGrupo() {
    if (!grupoExcluirId) return;

    try {
        const response = await fetch(
            `${GRUPO_API}/Deletar/${grupoExcluirId}`,
            {
                method: "DELETE",
                headers: getAuthHeaders()
            }
        );

        if (!response.ok) {
            mostrarToast("Erro ao excluir grupo", "danger");
            return;
        }

        modalConfirmacaoGrupo.hide();
        grupoExcluirId = null;

        mostrarToast("Grupo excluído com sucesso!");
        carregarGrupos();

    } catch (error) {
        console.error(error);
        mostrarToast("Erro inesperado", "danger");
    }
}
