import { useMemo, useState } from "react";
import {
  Link,
  Route,
  Routes,
  useLocation,
} from "react-router-dom";
import { pages } from "./content.js";
import { Logo } from "./components/Logo/Logo.jsx";
import { Sidebar } from "./components/Sidebar/Sidebar.jsx";
import { Header } from "./components/Header/Header.jsx";
import { DocPage } from "./components/DocPage/DocPage.jsx";
import { HomePage } from "./page/HomePage.jsx";
import { NotFound } from "./page/NotFound.jsx";

const featuredTags = [
  { title: "<text>", description: "Exiba texto e aplique estilos.", path: "/docs/tags/text", icon: "T" },
  { title: "<row>", description: "Organize conteúdo na horizontal.", path: "/docs/tags/row", icon: "↔" },
  { title: "<column>", description: "Organize conteúdo na vertical.", path: "/docs/tags/column", icon: "↕" },
  { title: "<box>", description: "Agrupe conteúdo com uma borda.", path: "/docs/tags/box", icon: "□" },
  { title: "<block>", description: "Crie um contêiner com título.", path: "/docs/tags/block", icon: "▣" },
  { title: "<hr>", description: "Adicione um separador visual.", path: "/docs/tags/hr", icon: "—" },
];

export default function App() {
  const [search, setSearch] = useState("");
  const [menuOpen, setMenuOpen] = useState(false);
  const location = useLocation();

  const currentPage = useMemo(() => pages[location.pathname], [location.pathname]);

  return (
    <div className="app-shell min-h-screen">
      <button
        aria-label="Fechar navegação"
        className={`mobile-scrim${menuOpen ? " mobile-scrim-visible" : ""}`}
        onClick={() => setMenuOpen(false)}
        type="button"
      />
      <div className={`sidebar-wrap${menuOpen ? " sidebar-wrap-open" : ""}`}>
        <div className="sidebar-brand"><Logo /></div>
        <Sidebar closeMenu={() => setMenuOpen(false)} search={search} setSearch={setSearch} />
      </div>
      <div className="main-column">
        <Header menuOpen={menuOpen} pageTitle={currentPage?.title} setMenuOpen={setMenuOpen} />
        <main className={location.pathname === "/" ? "main-content main-content-home" : "main-content"}>
          <Routes>
            <Route element={<HomePage tags={featuredTags} />} path="/" />
            {Object.entries(pages).map(([path, page]) => (
              <Route element={<DocPage key={path} page={page} />} key={path} path={path} />
            ))}
            <Route element={<NotFound />} path="*" />
          </Routes>
        </main>
      </div>
    </div>
  );
}
