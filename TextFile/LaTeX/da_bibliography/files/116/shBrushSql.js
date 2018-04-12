;(function()
{
	// CommonJS
	typeof(require) != 'undefined' ? SyntaxHighlighter = require('shCore').SyntaxHighlighter : null;

	function Brush()
	{
		var funcs	=	'abs acos ascii asin atan atn2 avg cast ceiling coalesce convert cos count count_big current_timestamp ' +
						'current_user dateadd datediff datename datepart day degrees dense_rank difference error_message exp ' +
					    'floor getdate getutcdate isnull log log10 lower ltrim month newid ntile nullif ' +
						'power quotename radians rand rank replace replicate reverse round pi row_number rtrim ' +
						'scope_identity serverproperty session_user sign sin soundex space sqrt square str ' +
						'substring sum system_user tan unicode upper user year';

		var keywords =	'absolute action add after alter and ansi_nulls as asc at authorization begin bigint ' +
						'binary bit break by cascade case catch char character check checkpoint close clustered collate ' +
						'column commit committed connect connection constraint contains continue ' +
						'create cube current current_date current_time cursor database date datetime dbcc ' +
						'deallocate dec decimal declare default delay delete desc disable distinct double drop ' +
						'dynamic else enable encryption end end-exec escape except exec execute false fetch fillfactor first ' +
						'float for force foreign forward free from full function global goto grant ' +
						'group grouping having hour identity identity_insert if ignore index inner insensitive insert instead ' +
						'int integer intersect into is isolation key last level load local login max min ' +
						'minute modify money move national nchar next no nocheck nocount nonclustered numeric of off on only ' +
						'open option or order out output over pad_index partial password persisted precision prepare primary print ' +
						'prior privileges procedure public quoted_identifier raiserror read real reconfigure references relative repeatable ' +
						'replication restrict return returns revoke rollback rollup rows rule save schema schemabinding scroll ' +
						'second section select sequence serializable set size smalldatetime smallint static ' +
						'statistics table temp temporary textimage_on then ties time timestamp to top tran transaction ' +
						'translation trigger true truncate try uncommitted union unique uniqueidentifier update use values ' +
						'varbinary varchar varying view waitfor when where while with work';

		var operators =	'all any between cross exists in join left like not null outer right some';

		this.regexList = [
			{ regex: SyntaxHighlighter.regexLib.multiLineCComments,				css: 'comments' },			// multiline comments
			{ regex: /(@@)\w+\b/gm,												css: 'color2' },			// one line and multiline comments
			{ regex: /(@)\w+\b/gm,												css: 'color1' },			// one line and multiline comments
			{ regex: /--(.*)$/gm,												css: 'comments' },			// one line and multiline comments
			{ regex: SyntaxHighlighter.regexLib.multiLineDoubleQuotedString,	css: 'string' },			// double quoted strings
			{ regex: SyntaxHighlighter.regexLib.multiLineSingleQuotedString,	css: 'string' },			// single quoted strings
			{ regex: new RegExp(this.getKeywords(funcs), 'gmi'),				css: 'color2' },			// functions
			{ regex: new RegExp(this.getKeywords(operators), 'gmi'),			css: 'color1' },			// operators and such
			{ regex: new RegExp(this.getKeywords(keywords), 'gmi'),				css: 'keyword' }			// keyword
			];
	};

	Brush.prototype	= new SyntaxHighlighter.Highlighter();
	Brush.aliases	= ['sql'];

	SyntaxHighlighter.brushes.Sql = Brush;

	// CommonJS
	typeof(exports) != 'undefined' ? exports.Brush = Brush : null;
})();

