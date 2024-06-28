# Caching

É uma técnica para armazenar dados, frequentemente usados, em uma memória local por um certo período de tempo. Com isso, da próxima vez que o usuário solicitar os mesmos dados não será necessário acessar o banco de dados para recuperar a informação, ela será retornada da memória local. Isto implica no aumento da performance, visto que acessar memória local é bem menos custoso que armazenar o banco de dados.

**Resumindo**: Cache é como uma camada de memória que armazena cópias de dados acessados com frequência.

## Caching local

Este tipo de cache é armazenado diretamente na memória da instância da aplicação. É muito rápido, mas pode ser desafiador de gerenciar em ambientes distribuídos, pois cada instância tem seu próprio cache local.

- Implementações comuns: Estruturas de dados in-memory como *HashMaps* ou *Caches* em bibliotecas.

## Cache Distribuído

Usado em ambientes distribuídos para garantir que múltiplas instâncias da aplicação possam acessar e compartilhar um cache comum. Este método ajuda a manter a consistência dos dados entre as instâncias.

- Implementações comuns:
    - Redis
    - MemCached

### **Cache de CDN (Content Delivery Network)**

Embora seja mais focado no frontend, o CDN pode ser considerado uma forma de caching para reduzir a latência de entrega de conteúdo estático (como imagens, JavaScript, CSS) para usuários ao redor do mundo.

- **Exemplos**: Cloudflare, Amazon CloudFront, Akamai.

### **→ Invalidação de Cache**

Uma das tarefas mais desafiadoras no caching é decidir quando um item no cache não é mais válido e deve ser atualizado ou removido. Estratégias comuns incluem:

- **Cache com tempo de vida (TTL)**: Onde os dados são automaticamente removidos após um período especificado.
- **Invalidação baseada em eventos**: Onde as atualizações nos dados subjacentes desencadeiam uma invalidação do cache relevante.

### **→Consistência**

Manter a consistência entre o cache e a fonte de dados é vital, especialmente em sistemas distribuídos onde várias instâncias podem modificar os dados. Estratégias como Write-Through e Write-Behind podem ajudar:

- **Write-Through**: Garante que as operações de escrita sejam refletidas no cache e na fonte de dados simultaneamente, oferecendo forte consistência.
- **Write-Behind**: As operações de escrita são primeiramente gravadas no cache e depois sincronizadas com a fonte de dados, o que pode melhorar a performance, mas com eventual consistência.
- Write-Through Cache: É um método onde as operações de escrita são direcionadas diretamente à fonte de dados principal, bypassando o cache. Os dados escritos não são cacheados até que sejam lidos pela primeira vez. Isso pode ajudar a evitar o preenchimento do cache com dados que talvez nunca sejam reutilizados, mantendo o cache mais livre para dados que são frequentemente acessados.

### Exemplo de Write-Through

**Cenário**: Um sistema de e-commerce que gerencia inventário em tempo real.

**Implementação**:

- **Operação**: Quando um cliente compra um item, o sistema deve atualizar o inventário.
- **Processo de Cache Write-Through**:
    1. O cliente finaliza a compra do item.
    2. A aplicação reduz a quantidade do item no inventário.
    3. A operação de atualização é enviada simultaneamente ao cache e ao banco de dados:
        - O cache é atualizado para refletir a nova quantidade disponível.
        - O banco de dados também é atualizado ao mesmo tempo.
    4. Isso garante que qualquer consulta subsequente ao cache refletirá imediatamente a quantidade correta, mantendo a consistência entre o cache e o banco de dados.

**Vantagem**: Garante que a quantidade mostrada no site é sempre precisa, evitando a venda de itens que já estão fora de estoque.

### Exemplo de Write-Around

**Cenário**: Um sistema de logging de eventos de uma aplicação.

**Implementação**:

- **Operação**: Eventos de log são gerados frequentemente por uma aplicação.
- **Processo de Cache Write-Around**:
    1. Cada evento de log é escrito diretamente no sistema de armazenamento (como um banco de dados ou sistema de arquivos), sem passar pelo cache.
    2. O cache não é atualizado com essas informações quando são escritas pela primeira vez.
    3. Quando um operador ou um sistema automatizado solicita um evento de log específico para análise, a aplicação verifica primeiro no cache.
    4. Se o evento não está no cache (cache miss), é recuperado do sistema de armazenamento e então cacheado. Operações futuras para o mesmo log serão mais rápidas, pois o dado já estará no cache.

**Vantagem**: Evita encher o cache com dados que não são frequentemente necessários (como logs antigos ou raramente acessados), maximizando o espaço do cache para dados que são mais propensos a serem reconsultados, mantendo o cache mais relevante e eficiente.

# Aplicação prática

Para exemplificar a utilização do cache, foi utilizado o cache mais simples do .Net que é o In-Memory Cache. O In-Memory Cache é um cache que armazena os dados em memória, ou seja, os dados são armazenados na memória do servidor.

Para utilizar o In-Memory Cache, é necessário adicionar a referência ao pacote Microsoft.Extensions.Caching.Memory.

A ideia é implementar tanto write-around quanto write-through cache. Para o write-around cache, a ideia é que os dados sejam armazenados no cache apenas quando são lidos. Para o write-through cache, a ideia é que os dados sejam armazenados no cache quando são escritos.

### LibraryService:

Então no GetLibrary foi utilizado o write-around cache, ou seja, os dados são armazenados no cache apenas quando são lidos, eles não são armazenados imediatamente quando adicionamos uma nova library. Com isso, apenas os dados requisitados são armazenados no cache. É necessário tomar cuidado com a invalidação do cache. Podemos invalidar o cache de duas formas: manualmente ou por tempo de vida. No exemplo, foi utilizado tanto o tempo de vida, ou seja, o cache é invalidado após 10 minutos, quanto a invalidação manual, ou seja, o cache é invalidado quando removemos a library.

Para o AddBook foi utilizado o write-through cache, ou seja, o objeto library com o novo livro é armazenado no cache e no banco de dados. Com isso, o cache é atualizado sempre que um novo livro é adicionado.
Já para o RemoveBook utilizamos a ideia de apenas invalidar o cache, com isso quando um livro é removido, o cache também é removido. Logo, quando tentarmos recuperar o livro é que ele não será encontrado no cache e será adicionado novamente.

Logo a escolha entre sempre salvar no cache as atualizações junto do banco de dados ou apenas invalidar o cache (apagando-o) e deixar que o cache seja preenchido novamente quando necessário, depende do sistema.


### Para rodar:

- Clone o repositório.
- Entre na pasta do projeto src/Api.
- Execute o comando dotnet ef migrations add InitialCreate para criar a migration.
- Execute o comando dotnet ef database update para criar o banco de dados (SQLITE).
- Execute o comando dotnet run para rodar a aplicação.
- Utilize o arquivo Api.http para realizar as requisições.