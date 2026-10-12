import { useEffect, useState } from 'react';
import { api } from './services/api';
import { StatusBadge } from './components/StatusBadge';
import { Activity, Shield, RefreshCw, AlertCircle } from 'lucide-react';

export interface InstrumentoResumo {
    id: string;
    codigo: string;
    tipo: string;
    ultimoStatus: string;
    ultimoValor: number | null;
    dataUltimaLeitura: string | null;
}

export interface BarragemStatus {
    barragemId: string;
    nomeBarragem: string;
    statusGlobal: string;
    totalInstrumentos: number;
    quantidadeNormal: number;
    quantidadeAtencao: number;
    quantidadeAlerta: number;
    quantidadeEmergencia: number;
    quantidadeSemLeitura: number;
    instrumentos: InstrumentoResumo[];
}

export interface Barragem {
    id: string;
    nome: string;
    localizacao: string;
}

export default function App() {
    const [barragens, setBarragens] = useState<Barragem[]>([]);
    const [consulta, setConsulta] = useState({
        barragemId: '',
        versao: 0,
    });

    const [resultado, setResultado] = useState<{
        barragemId: string;
        versao: number;
        dados: BarragemStatus | null;
        erro: string | null;
    } | null>(null);

    const [erroLista, setErroLista] = useState<string | null>(null);

    const { barragemId: barragemSelecionadaId, versao } = consulta;

    const resultadoAtual =
        resultado?.barragemId === barragemSelecionadaId &&
            resultado?.versao === versao
            ? resultado
            : null;

    const statusResumo = resultadoAtual?.dados ?? null;
    const erroApi = erroLista ?? resultadoAtual?.erro ?? null;
    const loading =
        Boolean(barragemSelecionadaId) && resultadoAtual === null;

    const setBarragemSelecionadaId = (barragemId: string) => {
        setConsulta((anterior) => ({
            barragemId,
            versao: anterior.versao + 1,
        }));
    };

    const carregarResumo = () => {
        if (!barragemSelecionadaId || loading) return;

        setConsulta((anterior) => ({
            ...anterior,
            versao: anterior.versao + 1,
        }));
    };

    // Carrega a lista de barragens.
    useEffect(() => {
        const controller = new AbortController();

        api.get<Barragem[]>('/Barragem', {
            signal: controller.signal,
        })
            .then((res) => {
                if (controller.signal.aborted) return;

                setBarragens(res.data);

                if (res.data.length > 0) {
                    setConsulta({
                        barragemId: res.data[0].id,
                        versao: 0,
                    });
                } else {
                    setErroLista('Nenhuma barragem cadastrada.');
                }
            })
            .catch((err) => {
                if (controller.signal.aborted) return;

                console.error('Erro ao carregar barragens:', err);
                setErroLista(
                    'Não foi possível carregar as barragens. Recarregue a página.'
                );
            });

        return () => controller.abort();
    }, []);

    // Carrega o resumo da seleção e atualização atuais.
    useEffect(() => {
        if (!barragemSelecionadaId) return;

        const controller = new AbortController();

        api.get<BarragemStatus>(
            `/Barragem/${barragemSelecionadaId}/status`,
            { signal: controller.signal }
        )
            .then((res) => {
                if (controller.signal.aborted) return;

                setResultado({
                    barragemId: barragemSelecionadaId,
                    versao,
                    dados: res.data,
                    erro: null,
                });
            })
            .catch((err) => {
                if (controller.signal.aborted) return;

                console.error('Erro ao carregar resumo:', err);
                setResultado({
                    barragemId: barragemSelecionadaId,
                    versao,
                    dados: null,
                    erro: 'Erro ao carregar o resumo. Tente atualizar novamente.',
                });
            });

        return () => controller.abort();
    }, [barragemSelecionadaId, versao]);

    return (
        <div className="min-h-screen bg-slate-950 text-slate-100 p-6">
            {/* Cabeçalho */}
            <header className="max-w-7xl mx-auto flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b border-slate-800 pb-6 mb-8">
                <div className="flex items-center gap-3">
                    <div className="p-2.5 bg-blue-600 rounded-xl shadow-lg shadow-blue-500/20">
                        <Activity className="w-6 h-6 text-white" />
                    </div>
                    <div>
                        <h1 className="text-2xl font-bold tracking-tight">Geotrend</h1>
                        <p className="text-xs text-slate-400">Sistema de Monitoramento Geotécnico de Barragens</p>
                    </div>
                </div>

                <div className="flex items-center gap-3">
                    <select
                        value={barragemSelecionadaId}
                        onChange={(e) => setBarragemSelecionadaId(e.target.value)}
                        className="bg-slate-900 border border-slate-700 text-slate-200 text-sm rounded-lg p-2.5 focus:ring-2 focus:ring-blue-500 outline-none min-w-[220px]"
                    >
                        {barragens.length === 0 ? (
                            <option value="">Carregando barragens...</option>
                        ) : (
                            barragens.map((b) => (
                                <option key={b.id} value={b.id}>{b.nome}</option>
                            ))
                        )}
                    </select>

                    <button
                        onClick={carregarResumo}
                        className="p-2.5 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-lg transition-colors border border-slate-700 cursor-pointer"
                        title="Atualizar Dados"
                    >
                        <RefreshCw className={`w-4 h-4 ${loading ? 'animate-spin' : ''}`} />
                    </button>
                </div>
            </header>

            {/* Alerta de Erro Visual */}
            {erroApi && (
                <div className="max-w-7xl mx-auto mb-6 p-4 bg-rose-500/10 border border-rose-500/20 rounded-xl text-rose-400 flex items-center gap-3 text-sm">
                    <AlertCircle className="w-5 h-5 shrink-0" />
                    <span>{erroApi}</span>
                </div>
            )}

            {/* Painel Principal */}
            {statusResumo ? (
                <main className="max-w-7xl mx-auto space-y-8">
                    <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-6 gap-4">
                        <div className="md:col-span-2 bg-slate-900 border border-slate-800 p-5 rounded-2xl flex flex-col justify-between">
                            <span className="text-xs font-medium text-slate-400 uppercase tracking-wider">Status Global da Estrutura</span>
                            <div className="mt-4 flex items-center justify-between">
                                <div>
                                    <h2 className="text-xl font-bold">{statusResumo.nomeBarragem}</h2>
                                    <p className="text-xs text-slate-500 mt-1">{statusResumo.totalInstrumentos} instrumentos cadastrados</p>
                                </div>
                                <StatusBadge status={statusResumo.statusGlobal} />
                            </div>
                            {statusResumo.quantidadeSemLeitura > 0 && (
                                <p className="text-xs text-slate-400 mt-3">
                                    Instrumentos sem leitura: {statusResumo.quantidadeSemLeitura}.
                                    {' '}O status considera somente as leituras disponíveis.
                                </p>
                            )}
                        </div>

                        <div className="bg-slate-900/60 border border-slate-800/80 p-4 rounded-2xl">
                            <span className="text-xs text-emerald-400 font-medium">Normal</span>
                            <p className="text-2xl font-bold mt-2">{statusResumo.quantidadeNormal}</p>
                        </div>
                        <div className="bg-slate-900/60 border border-slate-800/80 p-4 rounded-2xl">
                            <span className="text-xs text-amber-400 font-medium">Atenção</span>
                            <p className="text-2xl font-bold mt-2">{statusResumo.quantidadeAtencao}</p>
                        </div>
                        <div className="bg-slate-900/60 border border-slate-800/80 p-4 rounded-2xl">
                            <span className="text-xs text-rose-400 font-medium">Alerta / Emergência</span>
                            <p className="text-2xl font-bold mt-2">{statusResumo.quantidadeAlerta + statusResumo.quantidadeEmergencia}</p>
                        </div>
                        <div className="bg-slate-900/60 border border-slate-800/80 p-4 rounded-2xl">
                            <span className="text-xs text-slate-300 font-medium">Sem leitura</span>
                            <p className="text-2xl font-bold mt-2">{statusResumo.quantidadeSemLeitura}</p>
                        </div>
                    </div>

                    <div className="bg-slate-900 border border-slate-800 rounded-2xl overflow-hidden">
                        <div className="p-5 border-b border-slate-800 flex items-center justify-between">
                            <h3 className="font-semibold text-slate-200">Instrumentos Geotécnicos</h3>
                            <Shield className="w-4 h-4 text-slate-500" />
                        </div>
                        <div className="overflow-x-auto">
                            <table className="w-full text-left text-sm text-slate-400">
                                <thead className="bg-slate-950/50 text-xs uppercase text-slate-400 border-b border-slate-800">
                                    <tr>
                                        <th className="px-6 py-3.5">Código</th>
                                        <th className="px-6 py-3.5">Tipo</th>
                                        <th className="px-6 py-3.5">Última Leitura</th>
                                        <th className="px-6 py-3.5">Status</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-slate-800/60">
                                    {statusResumo.instrumentos.length === 0 && (
                                        <tr>
                                            <td colSpan={4} className="px-6 py-8 text-center text-slate-400">
                                                Nenhum instrumento cadastrado nesta barragem.
                                            </td>
                                        </tr>
                                    )}
                                    {statusResumo.instrumentos?.map((inst) => (
                                        <tr key={inst.id} className="hover:bg-slate-800/30 transition-colors">
                                            <td className="px-6 py-4 font-medium text-slate-200">{inst.codigo}</td>
                                            <td className="px-6 py-4">{inst.tipo}</td>
                                            <td className="px-6 py-4">
                                                {inst.ultimoValor !== null && inst.ultimoValor !== undefined ? (
                                                    <span className="text-slate-200 font-medium">{inst.ultimoValor}</span>
                                                ) : (
                                                    <span className="text-slate-600 italic">Sem leituras</span>
                                                )}
                                            </td>
                                            <td className="px-6 py-4">
                                                <StatusBadge status={inst.ultimoStatus} />
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </main>
            ) : (
                !erroApi && (
                    <div className="max-w-7xl mx-auto text-center py-20 text-slate-500">
                        <p>Carregando dados do sistema...</p>
                    </div>
                )
            )}
        </div>
    );
}
