import { Logo } from "../Logo/Logo.jsx";

export function Header({ menuOpen, setMenuOpen, pageTitle }) {
  return (
    <header className="topbar">
      <div className="topbar-mobile-brand">
        <Logo />
      </div>
      <div className="topbar-breadcrumb">
        <span>Documentação</span>
        <span className="breadcrumb-slash">/</span>
        <span className="breadcrumb-current">{pageTitle || "PTML"}</span>
      </div>
      <div className="topbar-actions">
        <a
          className="github-link"
          href="https://github.com/rafael-serafin1/ptml"
          rel="noreferrer"
          target="_blank"
        >
          <span aria-hidden="true">↗</span>
          <span>GitHub</span>
        </a>
        <button
          aria-expanded={menuOpen}
          aria-label={menuOpen ? "Fechar navegação" : "Abrir navegação"}
          className="menu-toggle"
          onClick={() => setMenuOpen(!menuOpen)}
          type="button"
        >
          <span />
          <span />
        </button>
      </div>
    </header>
  );
}