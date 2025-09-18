using Estoque.Application.Contracts;
using Estoque.Application.DTOs.Estoque;
using Estoque.Application.DTOs.Movimentacao;
using Estoque.Application.Mapping;
using Estoque.Domain.Enitities;
using Estoque.Domain.Repository;
using Shared.Common;
using Shared.Exceptions;
using Shared.Messaging.Abstractions;
using Shared.Messaging.Messages;
using System.Linq.Expressions;

namespace Estoque.Infrastructure.Services
{
    public class EstoqueService : IEstoqueService
    {
        private readonly IEstoqueRepository _estoqueRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IEventBus _eventBus;

        public EstoqueService(
            IEstoqueRepository estoqueRepository,
            IProdutoRepository produtoRepository,
            IEventBus eventBus)
        {
            _estoqueRepository = estoqueRepository;
            _produtoRepository = produtoRepository;
            _eventBus = eventBus;
        }

        public async Task<EstoqueDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var estoque = await _estoqueRepository.GetById(id, cancellationToken);
            if (estoque == null)
                throw new NotFoundException(nameof(EstoqueAggregate), id);

            var estoqueDTO = EstoqueMapper.ToDTO(estoque);
            return estoqueDTO;
        }

        public async Task<IEnumerable<EstoqueDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var estoques = await _estoqueRepository.GetAll(cancellationToken);
            var estoquesDTO = estoques.Select(e => EstoqueMapper.ToDTO(e)).ToList();
            return estoquesDTO;
        }

        public async Task<EstoqueDTO?> GetByProdutoIdAsync(int produtoId, CancellationToken cancellationToken = default)
        {
            var estoques = await _estoqueRepository.Find(e => e.ProdutoId == produtoId, cancellationToken);
            var estoque = estoques.FirstOrDefault();
            if (estoque == null)
                return null;

            var estoqueDTO = EstoqueMapper.ToDTO(estoque);
            return estoqueDTO;
        }

        public async Task<IEnumerable<EstoqueDTO>> FindAsync(Expression<Func<EstoqueDTO, bool>> predicate, CancellationToken cancellationToken = default)
        {
            var estoques = await _estoqueRepository.GetAll(cancellationToken);
            var estoquesDTO = estoques.Select(e => EstoqueMapper.ToDTO(e)).ToList();

            return estoquesDTO.AsQueryable().Where(predicate).ToList();
        }

        public async Task<EstoqueDTO> CreateAsync(CreateEstoqueDTO createDto, CancellationToken cancellationToken = default)
        {
            var produto = await _produtoRepository.GetById(createDto.produtoId, cancellationToken);

            if (produto == null)
                throw new NotFoundException(nameof(Produto), createDto.produtoId);

            var existingEstoques = await _estoqueRepository.Find(e => e.ProdutoId == createDto.produtoId, cancellationToken);
            if (existingEstoques.Any())
                throw new BusinessRuleException($"Já existe um estoque para o produto com ID {createDto.produtoId}");

            var estoque = new EstoqueAggregate(createDto.produtoId);
            await _estoqueRepository.Create(estoque, cancellationToken);

            estoque = await _estoqueRepository.GetById(estoque.Id, cancellationToken);
            return EstoqueMapper.ToDTO(estoque);
        }

        public async Task<EstoqueDTO> UpdateAsync(int id, UpdateEstoqueDTO updateDto, CancellationToken cancellationToken = default)
        {
            var estoque = await _estoqueRepository.GetById(id, cancellationToken);

            if (estoque == null)
                throw new NotFoundException(nameof(EstoqueAggregate), id);

            if (estoque.ProdutoId != updateDto.produtoId)
            {
                var newProduto = await _produtoRepository.GetById(updateDto.produtoId, cancellationToken);
                if (newProduto == null)
                    throw new NotFoundException(nameof(Produto), updateDto.produtoId);

                estoque.ProdutoId = updateDto.produtoId;
                await _estoqueRepository.Update(estoque, cancellationToken);
            }

            estoque = await _estoqueRepository.GetById(estoque.Id, cancellationToken);
            return EstoqueMapper.ToDTO(estoque);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var estoque = await _estoqueRepository.GetById(id, cancellationToken);

            if (estoque == null)
                return false;

            await _estoqueRepository.Delete(estoque, cancellationToken);
            return true;
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
        {
            var estoque = await _estoqueRepository.GetById(id, cancellationToken);
            return estoque != null;
        }

        public async Task<PagedResult<EstoqueDTO>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var allEstoques = await _estoqueRepository.GetAll(cancellationToken);
            var totalCount = allEstoques.Count();

            var pagedEstoques = allEstoques
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var mappedEstoques = pagedEstoques.Select(e => EstoqueMapper.ToDTO(e)).ToList();

            return new PagedResult<EstoqueDTO>
            {
                Items = mappedEstoques,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<EstoqueDTO> AdicionarEstoqueAsync(int estoqueId, int quantidade, string motivo, CancellationToken cancellationToken = default)
        {
            var estoque = await _estoqueRepository.GetById(estoqueId, cancellationToken);

            if (estoque == null)
                throw new NotFoundException(nameof(EstoqueAggregate), estoqueId);

            estoque.AdicionarEstoque(quantidade, motivo);
            await _estoqueRepository.Update(estoque, cancellationToken);

            _eventBus.Publish("stock.updated", new ProductStockUpdated
            {
                ProductId = estoque.ProdutoId,
                NewQuantity = estoque.Produto.QuantidadeEmEstoque,
                Timestamp = DateTime.UtcNow
            });

            return EstoqueMapper.ToDTO(estoque);
        }

        public async Task<EstoqueDTO> RemoverEstoqueAsync(int estoqueId, int quantidade, string motivo, CancellationToken cancellationToken = default)
        {
            var estoque = await _estoqueRepository.GetById(estoqueId, cancellationToken);

            if (estoque == null)
                throw new NotFoundException(nameof(EstoqueAggregate), estoqueId);

            try
            {
                estoque.RemoverEstoque(quantidade, motivo);
                await _estoqueRepository.Update(estoque, cancellationToken);

                _eventBus.Publish("stock.updated", new ProductStockUpdated
                {
                    ProductId = estoque.ProdutoId,
                    NewQuantity = estoque.Produto.QuantidadeEmEstoque,
                    Timestamp = DateTime.UtcNow
                });

                return EstoqueMapper.ToDTO(estoque);
            }
            catch (BusinessRuleException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Erro ao remover estoque", ex);
            }
        }

        public async Task<EstoqueDTO> AjustarEstoqueAsync(int estoqueId, int novaQuantidade, string motivo, CancellationToken cancellationToken = default)
        {
            var estoque = await _estoqueRepository.GetById(estoqueId, cancellationToken);

            if (estoque == null)
                throw new NotFoundException(nameof(EstoqueAggregate), estoqueId);

            estoque.AjustarEstoque(novaQuantidade, motivo);
            await _estoqueRepository.Update(estoque, cancellationToken);

            _eventBus.Publish("stock.updated", new ProductStockUpdated
            {
                ProductId = estoque.ProdutoId,
                NewQuantity = estoque.Produto.QuantidadeEmEstoque,
                Timestamp = DateTime.UtcNow
            });

            return EstoqueMapper.ToDTO(estoque);
        }

        public async Task<bool> VerificarEstoqueAbaixoDoMinimoAsync(int estoqueId, int quantidadeMinima, CancellationToken cancellationToken = default)
        {
            var estoque = await _estoqueRepository.GetById(estoqueId, cancellationToken);

            if (estoque == null)
                throw new NotFoundException(nameof(EstoqueAggregate), estoqueId);

            return estoque.EstoqueAbaixoDoMinimo(quantidadeMinima);
        }

        public async Task<IEnumerable<MovimentacaoDTO>> ObterHistoricoMovimentacoesAsync(
            int estoqueId,
            DateTime? dataInicial = null,
            DateTime? dataFinal = null,
            CancellationToken cancellationToken = default)
        {
            var estoque = await _estoqueRepository.GetById(estoqueId, cancellationToken);

            if (estoque == null)
                throw new NotFoundException(nameof(EstoqueAggregate), estoqueId);

            var movimentacoes = estoque.ObterHistoricoMovimentacoes(dataInicial, dataFinal);
            return movimentacoes.Select(m => MovimentacaoMapper.ToDTO(m)).ToList();
        }
    }
}