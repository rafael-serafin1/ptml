import { navigation } from "../../content.js";
import { NavLink } from "react-router-dom";

export function Sidebar({ search, setSearch, closeMenu }) {
  const query = search.trim().toLocaleLowerCase("pt-BR");

  return (
    <aside className="sidebar">
      <div className="sidebar-search">
        <label className="sr-only" htmlFor="docs-search">Buscar na documentação</label>
        <span aria-hidden="true" className="search-icon">⌕</span>
        <input
          autoComplete="off"
          id="docs-search"
          onChange={(event) => setSearch(event.target.value)}
          placeholder="Buscar..."
          type="search"
          value={search}
        />
      </div>
      <nav aria-label="Navegação da documentação" className="sidebar-nav">
        {navigation.map((group) => {
          const items = group.items.filter((item) =>
            item.title.toLocaleLowerCase("pt-BR").includes(query),
          );
          if (query && items.length === 0) return null;

          return (
            <section className="nav-group" key={group.label}>
              <h2>{group.label}</h2>
              {items.map((item) => (
                <NavLink
                  className={({ isActive }) =>
                    `nav-link${isActive ? " nav-link-active" : ""}`
                  }
                  key={item.path}
                  onClick={closeMenu}
                  to={item.path}
                >
                  {item.title}
                  {item.path.startsWith("/docs/tags/") && (
                    <span className="nav-link-arrow" aria-hidden="true">↗</span>
                  )}
                </NavLink>
              ))}
            </section>
          );
        })}
        {query && navigation.every((group) =>
          group.items.every((item) =>
            !item.title.toLocaleLowerCase("pt-BR").includes(query),
          ),
        ) && <p className="search-empty">Nenhuma página encontrada.</p>}
      </nav>
      <div className="sidebar-note">
        <span className="note-indicator" />
        <p>Documentação baseada nos elementos confirmados no parser e no renderizador.</p>
      </div>
    </aside>
  );
}