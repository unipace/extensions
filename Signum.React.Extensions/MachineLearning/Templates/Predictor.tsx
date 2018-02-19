﻿import * as React from 'react'
import { Tabs, Tab } from 'react-bootstrap';
import * as numbro from 'numbro';
import * as OrderUtils from '../../../../Framework/Signum.React/Scripts/Frames/OrderUtils'
import { classes } from '../../../../Framework/Signum.React/Scripts/Globals'
import { FormGroup, FormControlStatic, ValueLine, ValueLineType, EntityLine, EntityDetail, EntityCombo, EntityList, EntityRepeater, EntityTable, IRenderButtons, EntityTabRepeater } from '../../../../Framework/Signum.React/Scripts/Lines'
import { SearchControl, FilterOption, ColumnOption, FindOptions } from '../../../../Framework/Signum.React/Scripts/Search'
import { TypeContext, FormGroupStyle, ButtonsContext } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import FileLine from '../../Files/FileLine'
import { PredictorEntity, PredictorColumnEmbedded, PredictorMessage, PredictorSubQueryEntity, PredictorFileType, PredictorCodificationEntity, PredictorSubQueryColumnEmbedded, PredictorEpochProgressEntity, NeuralNetworkSettingsEntity } from '../Signum.Entities.MachineLearning'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import { getQueryNiceName } from '../../../../Framework/Signum.React/Scripts/Reflection'
import QueryTokenEntityBuilder from '../../UserAssets/Templates/QueryTokenEntityBuilder'
import { QueryFilterEmbedded } from '../../UserQueries/Signum.Entities.UserQueries'
import { QueryDescription, SubTokensOptions } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import * as PredictorClient from '../PredictorClient';
import { toLite } from "../../../../Framework/Signum.React/Scripts/Signum.Entities";
import FilterBuilder from '../../../../Framework/Signum.React/Scripts/SearchControl/FilterBuilder';
import { MList, newMListElement } from '../../../../Framework/Signum.React/Scripts/Signum.Entities';
import FilterBuilderEmbedded from './FilterBuilderEmbedded';
import PredictorSubQuery from './PredictorSubQuery';
import { QueryTokenEmbedded } from '../../UserAssets/Signum.Entities.UserAssets';
import { QueryEntity } from '../../../../Framework/Signum.React/Scripts/Signum.Entities.Basics';
import { FilePathEmbedded } from '../../Files/Signum.Entities.Files';
import { is } from '../../../../Framework/Signum.React/Scripts/Signum.Entities';
import ProgressBar from './ProgressBar'
import LineChart, { LineChartSerie } from './LineChart'
import { QueryToken } from '../../../../Framework/Signum.React/Scripts/FindOptions';
import PredictorMetrics from './PredictorMetrics';
import PredictorClassificationMetrics from './PredictorClassificationMetrics';
import PredictorRegressionMetrics from './PredictorRegressionMetrics';

export default class Predictor extends React.Component<{ ctx: TypeContext<PredictorEntity> }, { queryDescription?: QueryDescription }> implements IRenderButtons {

    handleClick = () => {

        var p = this.props.ctx.value;

        if (!p.mainQuery.groupResults) {

            Finder.find({
                queryName: this.state.queryDescription!.queryKey,
                columnOptionsMode: "Add",
                columnOptions: p.mainQuery.columns.map(mle => ({ columnName: mle.element.token && mle.element.token.token!.fullKey }) as ColumnOption)
            })
                .then(lite => PredictorClient.predict(toLite(p), lite && { "Entity" : lite }))
                .done();

        } else {

            var fullKeys = p.mainQuery.columns.map(mle => mle.element.token!.tokenString!);

            Finder.findRow({
                queryName: this.state.queryDescription!.queryKey,
                groupResults: p.mainQuery.groupResults,
                columnOptionsMode: "Replace",
                columnOptions: fullKeys.map(fk => ({ columnName: fk }) as ColumnOption)
            }, { searchControlProps: { allowChangeColumns: false, showGroupButton: false } })
                .then(row => PredictorClient.predict(toLite(p), row && fullKeys.map((fk, i) => ({ tokenString: fk, value: row!.columns[i] })).toObject(a => a.tokenString, a => a.value)))
                .done();
        }
    }

    renderButtons(ctx: ButtonsContext): (React.ReactElement<any> | undefined)[] {
        if ((ctx.pack.entity as PredictorEntity).state == "Trained") {
            return [OrderUtils.setOrder(10000, <button className="btn btn-info" onClick={this.handleClick}><i className="fa fa-lightbulb-o"></i>&nbsp;{PredictorMessage.Predict.niceToString()}</button >)];
        } else {
            return [];
        }
    }

    constructor(props: any) {
        super(props);
        this.state = { queryDescription: undefined };
    }

    componentWillMount() {

        let p = this.props.ctx.value;
        if (p.mainQuery.query)
            this.loadData(p.mainQuery.query);
    }

    loadData(query: QueryEntity) {
        Finder.getQueryDescription(query.key)
            .then(qd => this.setState({ queryDescription: qd }))
            .done();
    }


    handleQueryChange = () => {

        const p = this.props.ctx.value;
        p.mainQuery.filters.forEach(a => a.element.token = fixTokenEmbedded(a.element.token || null, p.mainQuery.groupResults || false));
        p.mainQuery.columns.forEach(a => a.element.token = fixTokenEmbedded(a.element.token || null, p.mainQuery.groupResults || false));
        this.forceUpdate();

        this.setState({
            queryDescription: undefined
        }, () => {
            if (p.mainQuery.query)
                this.loadData(p.mainQuery.query);
        });
    }

    handleCreate = () => {

        var mq = this.props.ctx.value.mainQuery;

        var promise = !mq.groupResults ? Finder.parseSingleToken(mq.query!.key, "Entity", SubTokensOptions.CanElement).then(t => [t]) :
            Promise.resolve(mq.columns.map(a => a.element.token).filter(t => t != null && t.token != null && t.token.queryTokenType != "Aggregate").map(t => t!.token!));


        return promise.then(keys => PredictorSubQueryEntity.New({
            query: mq.query,
            columns: keys.map(t =>
                newMListElement(PredictorSubQueryColumnEmbedded.New({
                    usage: "ParentKey",
                    token: QueryTokenEmbedded.New({
                        token: t,
                        tokenString: t.fullKey,
                    })
                }))
            )
        }));
    }

    handleAlgorithmChange = () => {
        var pred = this.props.ctx.value;
        var al = pred.algorithm;
        if (al == null)
            pred.algorithmSettings = null;
        else {
            var init = PredictorClient.initializers[al.key];

            if (init != null)
                init(pred);
        }

        this.forceUpdate();
    }

    handleOnFinished = () => {
        const ctx = this.props.ctx;
        Navigator.API.fetchEntityPack(toLite(ctx.value))
            .then(pack => ctx.frame!.onReload(pack))
            .done();
    }

    handlePreviewMainQuery = (e: React.MouseEvent<any>) => {
        e.preventDefault();
        e.persist();
        var mq = this.props.ctx.value.mainQuery;

        var canAggregate = mq.groupResults ? SubTokensOptions.CanAggregate : 0;

        FilterBuilderEmbedded.toFilterOptionParsed(this.state.queryDescription!, mq.filters, SubTokensOptions.CanElement | SubTokensOptions.CanAnyAll | canAggregate)
            .then(filters => {
                var fo: FindOptions = {
                    queryName: mq.query!.key,
                    groupResults: mq.groupResults,
                    filterOptions: filters.map(f => ({
                        columnName: f.token!.fullKey,
                        operation: f.operation,
                        value: f.value
                    }) as FilterOption),
                    columnOptions: mq.columns.orderBy(mle => mle.element.usage == "Input" ? 0 : 1).map(mle => ({
                        columnName: mle.element.token && mle.element.token.tokenString,
                    } as ColumnOption)),
                    columnOptionsMode: "Replace",
                };

                Finder.exploreWindowsOpen(fo, e);
            })
            .done();
    }

    render() {
        let ctx = this.props.ctx;

        if (ctx.value.state != "Draft")
            ctx = ctx.subCtx({ readOnly: true });

        const ctxxs = ctx.subCtx({ formGroupSize: "ExtraSmall" });
        const ctxxs4 = ctx.subCtx({ labelColumns: 4 });
        const ctxmq = ctxxs.subCtx(a => a.mainQuery);
        const entity = ctx.value;
        const queryKey = entity.mainQuery.query && entity.mainQuery.query.key;

        var canAggregate = entity.mainQuery.groupResults ? SubTokensOptions.CanAggregate : 0;

        return (
            <div>
                <div className="row">
                    <div className="col-sm-6">
                        <ValueLine ctx={ctxxs4.subCtx(e => e.name)} readOnly={this.props.ctx.readOnly} />
                        <ValueLine ctx={ctxxs4.subCtx(e => e.state, { readOnly: true })} />
                        <EntityLine ctx={ctxxs4.subCtx(e => e.trainingException, { readOnly: true })} hideIfNull={true} />
                    </div>
                    <div className="col-sm-6">
                        <EntityCombo ctx={ctxxs4.subCtx(f => f.algorithm)} onChange={this.handleAlgorithmChange} />
                        <EntityCombo ctx={ctxxs4.subCtx(f => f.resultSaver)} />
                        <EntityCombo ctx={ctxxs4.subCtx(f => f.publication)} readOnly={true} />
                    </div>
                </div>
                {ctx.value.state == "Training" && <TrainingProgressComponent ctx={ctx} onStateChanged={this.handleOnFinished} />}
                <Tabs id={ctx.prefix + "tabs"} unmountOnExit={true}>
                    <Tab eventKey="query" title={ctxmq.niceName(a => a.query)}>
                        <div>
                            <fieldset>
                                <legend>{ctxmq.niceName()}</legend>
                                <EntityLine ctx={ctxmq.subCtx(f => f.query)} remove={ctx.value.isNew} onChange={this.handleQueryChange} />
                                {queryKey && <div>
                                    <ValueLine ctx={ctxmq.subCtx(f => f.groupResults)} onChange={this.handleQueryChange} />

                                    <FilterBuilderEmbedded ctx={ctxmq.subCtx(a => a.filters)}
                                        queryKey={queryKey}
                                        subTokenOptions={SubTokensOptions.CanAnyAll | SubTokensOptions.CanElement | canAggregate} />
                                    <EntityTable ctx={ctxmq.subCtx(e => e.columns)} columns={EntityTable.typedColumns<PredictorColumnEmbedded>([
                                        { property: a => a.usage },
                                        {
                                            property: a => a.token,
                                            template: (cctx, row) => <QueryTokenEntityBuilder
                                                ctx={cctx.subCtx(a => a.token)}
                                                queryKey={this.props.ctx.value.mainQuery.query!.key}
                                                subTokenOptions={SubTokensOptions.CanElement | canAggregate}
                                                onTokenChanged={() => { initializeColumn(ctx.value, cctx.value); row.forceUpdate() }} />,
                                            headerHtmlAttributes: { style: { width: "40%" } },
                                        },
                                        { property: a => a.encoding },
                                        { property: a => a.nullHandling },
                                    ])} />
                                    {ctxmq.value.query && <a href="#" onClick={this.handlePreviewMainQuery}>{PredictorMessage.Preview.niceToString()}</a>}
                                </div>}

                            </fieldset>
                            {queryKey && <EntityTabRepeater ctx={ctxxs.subCtx(e => e.subQueries)} onCreate={this.handleCreate}
                                getTitle={(mctx: TypeContext<PredictorSubQueryEntity>) => mctx.value.name || PredictorSubQueryEntity.niceName()}
                                getComponent={(mctx: TypeContext<PredictorSubQueryEntity>) =>
                                    <div>
                                        {!this.state.queryDescription ? undefined : <PredictorSubQuery ctx={mctx} mainQuery={ctxmq.value} mainQueryDescription={this.state.queryDescription} />}
                                    </div>
                                } />}
                        </div>
                    </Tab>
                    <Tab eventKey="settings" title={ctxxs.niceName(a => a.settings)}>
                        {ctxxs.value.algorithm && <EntityDetail ctx={ctxxs.subCtx(f => f.algorithmSettings)} remove={false} />}
                        <EntityDetail ctx={ctxxs.subCtx(f => f.settings)} remove={false} />
                    </Tab>
                    {
                        ctx.value.state != "Draft" && <Tab eventKey="codifications" title={PredictorMessage.Codifications.niceToString()}>
                            <SearchControl findOptions={{ queryName: PredictorCodificationEntity, parentColumn: "Predictor", parentValue: ctx.value }} />
                        </Tab>
                    }
                    {
                        ctx.value.state != "Draft" && <Tab eventKey="progress" title={PredictorMessage.Progress.niceToString()}>
                            {ctx.value.state == "Trained" && <EpochProgressComponent ctx={ctx} />}
                            <SearchControl findOptions={{ queryName: PredictorEpochProgressEntity, parentColumn: "Predictor", parentValue: ctx.value }} />
                        </Tab>
                    }
                    {
                        ctx.value.state == "Trained" && <Tab eventKey="files" title={PredictorMessage.Results.niceToString()}>
                            {ctx.value.resultTraining && ctx.value.resultValidation && <PredictorMetrics ctx={ctx} />}
                            {ctx.value.classificationTraining && ctx.value.classificationValidation && <PredictorClassificationMetrics ctx={ctx} />}
                            {ctx.value.regressionTraining && ctx.value.regressionTraining && <PredictorRegressionMetrics ctx={ctx} />}
                            {ctx.value.resultSaver && PredictorClient.getResultRendered(ctx)}
                            <div className="form-vertical">
                                <EntityRepeater ctx={ctxxs.subCtx(f => f.files)} getComponent={ec =>
                                    <FileLine ctx={ec.subCtx({ formGroupStyle: "SrOnly" })} remove={false} fileType={PredictorFileType.PredictorFile} />
                                } />
                            </div>
                        </Tab>
                    }
                </Tabs>
            </div>
        );
    }
}

export function initializeColumn(p: PredictorEntity, pc: PredictorColumnEmbedded | PredictorSubQueryColumnEmbedded) {
    var token = pc.token && pc.token.token;
    if (token) {
        pc.encoding =
            token.type.name == "number" || token.type.name == "decimal" ? "NormalizeZScore" :
                NeuralNetworkSettingsEntity.isInstance(p.algorithmSettings) ? (token.type.name == "boolean" ? "None" : "OneHot") :
                    "Codified";

        pc.nullHandling = "Zero";
    }
}


interface TrainingProgressComponentProps {
    ctx: TypeContext<PredictorEntity>;
    onStateChanged: () => void;
}

interface TrainingProgressComponentState {
    trainingProgress?: PredictorClient.TrainingProgress | null;
}

export class TrainingProgressComponent extends React.Component<TrainingProgressComponentProps, TrainingProgressComponentState> {

    constructor(props: TrainingProgressComponentProps) {
        super(props);
        this.state = {};
    }

    componentWillMount() {
        this.loadData(this.props);
    }

    componentWillReceiveProps(newProps: TrainingProgressComponentProps) {
        if (!is(newProps.ctx.value, this.props.ctx.value))
            this.loadData(newProps);
    }

    componentWillUnmount() {
        if (this.timeoutHandler)
            clearTimeout(this.timeoutHandler);
    }

    refreshInterval = 500;

    timeoutHandler!: number;

    loadData(props: TrainingProgressComponentProps) {
        PredictorClient.API.getTrainingState(toLite(props.ctx.value))
            .then(p => {
                var prev = this.state.trainingProgress;
                this.setState({ trainingProgress: p });
                if (prev != null && prev.State != p.State)
                    this.props.onStateChanged();
                else
                    this.timeoutHandler = setTimeout(() => this.loadData(this.props), this.refreshInterval);
            })
            .done();
    }

    render() {

        const tp = this.state.trainingProgress;

        return (
            <div>
                {tp && tp.EpochProgressesParsed && <LineChart height={200} series={getSeries(tp.EpochProgressesParsed, this.props.ctx.value)} />}
                <ProgressBar color={tp == null || tp.Running == false ? "info" : "default"}
                    value={tp && tp.Progress}
                    message={tp == null ? PredictorMessage.StartingTraining.niceToString() : tp.Message}
                />
            </div>
        );
    }

}


interface EpochProgressComponentProps {
    ctx: TypeContext<PredictorEntity>;
}

interface EpochProgressComponentState {
    epochProgress?: PredictorClient.EpochProgress[] | null;
}

export class EpochProgressComponent extends React.Component<EpochProgressComponentProps, EpochProgressComponentState> {

    constructor(props: EpochProgressComponentProps) {
        super(props);
        this.state = {};
    }

    componentWillMount() {
        this.loadData(this.props);
    }

    componentWillReceiveProps(newProps: EpochProgressComponentProps) {
        if (!is(newProps.ctx.value, this.props.ctx.value))
            this.loadData(newProps);
    }

    loadData(props: EpochProgressComponentProps) {
        PredictorClient.API.getEpochLosses(toLite(props.ctx.value))
            .then(p => {
                var prev = this.state.epochProgress;
                this.setState({ epochProgress: p });
            })
            .done();
    }

    render() {
        const eps = this.state.epochProgress;

        return (
            <div>
                {eps && <LineChart height={200} series={getSeries(eps, this.props.ctx.value)} />}
            </div>
        );
    }

}

function getSeries(eps: Array<PredictorClient.EpochProgress>, predictor: PredictorEntity): LineChartSerie[] {

    const algSet = predictor.algorithmSettings;

    const isClassification = NeuralNetworkSettingsEntity.isInstance(algSet) && algSet.predictionType == "Classification";

    var totalMax = isClassification ? undefined : eps.flatMap(a => [a.LossTraining, a.LossValidation]).filter(a => a != null).max();

    return [
        {
            color: "black",
            name: PredictorEpochProgressEntity.nicePropertyName(a => a.lossTraining),
            values: eps.filter(a => a.LossTraining != null).map(ep => ({ x: ep.TrainingExamples, y: ep.LossTraining })),
            minValue: 0,
            maxValue: totalMax,
            strokeWidth: "2px",
        },
        {
            color: "darkgray",
            name: PredictorEpochProgressEntity.nicePropertyName(a => a.evaluationTraining),
            values: eps.filter(a => a.EvaluationTraining != null).map(ep => ({ x: ep.TrainingExamples, y: ep.EvaluationTraining })),
            minValue: 0,
            maxValue: isClassification ? 1 : totalMax,
            strokeWidth: "1px",
        },
        {
            color: "red",
            name: PredictorEpochProgressEntity.nicePropertyName(a => a.lossValidation),
            values: eps.filter(a => a.LossValidation != null).map(ep => ({ x: ep.TrainingExamples, y: ep.LossValidation! })),
            minValue: 0,
            maxValue: totalMax,
            strokeWidth: "2px",
        },
        {
            color: "pink",
            name: PredictorEpochProgressEntity.nicePropertyName(a => a.evaluationValidation),
            values: eps.filter(a => a.EvaluationValidation != null).map(ep => ({ x: ep.TrainingExamples, y: ep.EvaluationValidation! })),
            minValue: 0,
            maxValue: isClassification ? 1 : totalMax,
            strokeWidth: "1px",
        }
    ];
}

function fixTokenEmbedded(token: QueryTokenEmbedded | null, groupResults: boolean): QueryTokenEmbedded | null {
    if (token == undefined)
        return null;

    const t = token.token!;

    const ft = fixToken(t, groupResults);

    if (t == ft)
        return token;

    if (ft == null)
        return null;

    return QueryTokenEmbedded.New({ token: ft, tokenString: ft.fullKey });

}

function fixToken(token: QueryToken | null, groupResults: boolean): QueryToken | null {
    if (token == undefined)
        return null;

    if (groupResults && token.queryTokenType == "Aggregate")
        return token.parent || null;

    return token;
}