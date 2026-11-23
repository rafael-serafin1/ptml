import { Link } from "react-router-dom";

export function HomePage({ tags }) {
  return (
    <div className="home-page">
      <div className="hero">
        <div className="eyebrow"><span />DOCUMENTAÇÃO</div>
        <h1>Interfaces de terminal,<br /><span>com estrutura.</span></h1>
        <p>
          PTML é uma linguagem declarativa para criar interfaces de terminal
          legíveis, organizadas e fáceis de compor.
        </p>
        <div className="hero-actions">
          <Link className="button-primary focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-green-700" to="/docs/introducao">
            Começar a aprender <span aria-hidden="true">→</span>
          </Link>
          <Link className="button-secondary focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-green-700" to="/docs/tags/text">
            Explorar elementos
          </Link>
        </div>
        <div className="hero-terminal">
          <div className="terminal-top">
            <span className="terminal-dots"><i /><i /><i /></span>
            <span>primeiro-exemplo.ptml</span>
            <span className="terminal-lang">PTML</span>
          </div>
            <pre>
              <code>
                <span className = "code-muted">&lt;</span>
                <span className = "code-tag">column</span>
                <span className = "code-attr"> gap</span>=<span className="code-string">"1"</span><span className = "code-muted">&gt;</span>
                {"\n"}  
                  <span className = "code-muted">&lt;</span><span className="code-tag">block</span> <span className = "code-attr"> title</span>=<span className="code-string">"Status"</span><span className = "code-muted">&gt;</span>
                  {"\n"}    
                  <span className = "code-muted">&lt;</span><span className="code-tag">text</span> <span className = "code-attr">foreground</span> = <span className = "code-string">"limegreen"</span><span className="code-muted">&gt;</span>
                  {"\n"}      Servidor online{"\n"}    
                  <span className = "code-muted">&lt;/</span><span className = "code-tag">text</span><span className = "code-muted">&gt;</span>
                  {"\n"}  <span className = "code-muted">&lt;/</span>
                  <span className = "code-tag">block</span><span className = "code-muted">&gt;</span>
                {"\n"}<span className = "code-muted">&lt;/</span><span className = "code-tag">column</span><span className = "code-muted">&gt;</span>
              </code>
            </pre>
            <div  className = "terminal-status"><span /> marcação declarativa para o seu terminal</div>
        </div>
      </div>

      <section className="home-section">
        <div className="section-heading">
          <div>
            <div className="eyebrow"><span />NAVEGUE PELA DOCUMENTAÇÃO</div>
            <h2>O essencial para começar</h2>
          </div>
          <Link to="/docs/sintaxe">Ver sintaxe <span aria-hidden="true">→</span></Link>
        </div>
        <div className="tag-grid">
          {tags.map((tag) => (
            <Link className="tag-card" key={tag.path} to={tag.path}>
              <span className="tag-card-icon">{tag.icon}</span>
              <span className="tag-card-title">{tag.title}<span aria-hidden="true">↗</span></span>
              <span className="tag-card-description">{tag.description}</span>
            </Link>
          ))}
        </div>
      </section>

      <section className="home-callout">
        <div className="callout-mark">&lt;/&gt;</div>
        <div>
          <h2>Um primeiro passo simples</h2>
          <p>Comece pelo texto e componha sua interface com os elementos documentados.</p>
        </div>
        <Link to="/docs/introducao">Ler introdução <span aria-hidden="true">→</span></Link>
      </section>
      <p className="home-footnote">Uma linguagem de marcação para o terminal. Não um substituto para linguagens de programação.</p>
    </div>
  );
}