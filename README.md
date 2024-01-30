## Expression builder. Dynamically build arbitrary lambda expressions directly from source code

Available as [Nuget package](https://www.nuget.org/packages/ExpressionBuilder/)

A fluent interface to create Lambda functions and expression.
The documentation can be found on http://www.kendar.org/?p=/dotnet/expressionsbuilder.
No knowledge of IL and System.Linq.Expression is needed. And even Lambda can be called.
Available items are: Function parameters and Variables, Constants, String functions,
Invocation of static and instance methods, If and While statements, Assignament,
Conditions, Cast, New instance of arbitrary types. For debugging purposes it's possible
to generate the source code for the function just created.

## If you like it Buy me a coffe :)

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/paypalme/kendarorg/1)

### Introduction

Recently I had to create lambda functions on the fly. I could'nt use the various code providers like the CSharpCodeProvider, since there would have been too much code to write. So i started to explore the wonders of Expressions. But it's a steep hill to climb.

At the same time i were exploring fluent interfaces, then i managed to build this library with a somehow fluent interface.

### Using the code

#### Creating a function

To define a new function these are the steps. We are creating a simple function, with a parameter and a return variable like

<pre>
int MyFunction(int param)
{
	return param;
}
</pre>

* Function.Create(): The starting point
* WithParameter<ParameterType>("Parameter name"): A parameter is declared. Multiple parameters can be declared
* Returns("Return variable name"): The variable that will be returned 

<pre>
var newFunction = Function.Create()
		.WithParameter&lt;int>("param")
		.Returns("param")
</pre>

Than we can convert it to an expression and call it. The compile step will produce a method.

<pre>
var newExpression = newFunction.ToExpression();
var method = newExpression.Compile();

var result = (int)method.invoke(2);
Assert.AreTrue(2,result);
</pre>

Or it's possible to build a lambda expression. Than it would be possible to call it with compilation support

<pre>
var lambda = newFunction.ToLambda&lt;Func&lt;int,int>>();

var result = lambda(2);
Assert.AreTrue(2,result);
</pre>

Of course it's possible to omit parameters and return variable declarations.

In short, in order of possible usage
* Function.Create(): Create a new function
* WithParameter: Create a parameter with the given type and name
	* WithParameter<DataType>("name")
	* WithParameter(typeof(DataType),"name")
* Returns("name"): Return the value of the variable
* In alternative
	* ToExpression(): Return a System.Linq.Expression, that should be compiled before the usage
	* ToLambda<LambdaType>(): Creates a lambda, ready to use
	* ToString(): Create the pseudo code for the function
	
#### Function body

A body can be declared for the function. It will contain a series of CodeLine objects. CodeLine objects into the library implement the ICodeLine interface

<pre>
var newFunction = Function.Create()
		.WithParameter&lt;int>("param")
		.WithBody(
			//A list of CodeLines
		)
		.Returns("param")
</pre>

In short, in order of possible usage
* Function.Create
* WithParameter
* WithBody(...): Setup the body of the function
* Returns

#### CodeLines

##### Variables

It is possible to declare new simple variables. They will be assigned automatically with a default value. Here we create a variable of type int named intVariable, with the default value of 0.

<pre>
var newFunction = Function.Create()
	...
	.WithBody(
		...
        CodeLine.CreateVariable&lt;int>("intVariable"),
		...
	)
	...
</pre>

Two methods can be used

* CreateVariable&lt;VariableType>("name")
* CreateVariable(typeof(VariableType),"name")

##### Assignment and constants

Values can be assigned to variables.

Let's start remembering that with assignments three roles are present

* L(eft) Value: The item receiving the value
* Operator: The assignment operator
* R(ight) Value: The source value

Inside the library the LValue and RValue are associated with two interfaces, and are produced by the "Operation" classes

* ILeftable: An object that can receive values, like the variables
* IRightable: An object that can provide values, like variables, constants and functions

For example let's assign to the variable intVariable a constant value with the AssignConst method. Then we will assign the value of intVariable to anotherVariable. Code line are not ILeftable or IRightable

<pre>
var newFunction = Function.Create()
	...
	.WithBody(
		...
        CodeLine.CreateVariable&lt;int>("intVariable"),
        CodeLine.CreateVariable&lt;int>("anotherVariable"),
		CodeLine.AssignConstant("intVariable",2),
		CodeLine.Assign("anotherVariable","intVariable")
		...
	)
	...
</pre>

The assignement can be made with several operations. The default is AssignementOperator.Assign in total they are

* Assign
* SumAssign
* SubtractAssign: Does not work for strings
* MultiplyAssign: Does not work for strings

While the complete CodeLines are

* AssignConstant
	* AssignConstant("name",value,operator): Assign the value to the "name" variable
	* AssignConstant(ILeftable,value,operator): Assign the value to the ILeftable variable
* Assign
	* Assign("name1","name2",operator)
	* Assign(ILeftable,"name2",operator)
	* Assign("name1",IRightable,operator)
	* Assign(ILeftable,IRightable,operator)

##### Operation

A particular concept is the "Operation". We will add the L or R values into the description to specify where an operation can be placed. The type IOperation means that the value can be both an ILeftable or an IRightable

* Variable("VariableName"): (LR) retrieves a variable instance given the name
* Constant(value): (R) generate an instance of a constant value
* Null: (R) The null constant
* Cast: (R) To cast the operation passed to it
	* Cast("name",toType)
	* Cast(IOperation,toType)
	* Cast&lt;toType>("name")
	* Cast&lt;toType>(IOperation)
	* CastConst(value,toType)
	* CastConst&lt;toType>(value)
* Invoke: (-) Invoke a method with a void return type. This is the only Operation that is not rightable or leftable
	* Invoke("name","MethodName",IOperation[]): Invoke the MethodName method on the "name" variable passing the IOperation parameters
	* Invoke(IOperation,"MethodName",IOperation[]): Using the IOperation variable
	* Invoke&lt;Type>("MethodName",IOperation[]): Invoke a static method on type
	* Invoke(Type,"MethodName",IOperation[]): Invoke a static method on type
* InvokeReturn: (R) Invoke a method with a return type
	* InvokeReturn("name","MethodName",IOperation[]): Invoke the MethodName method on the "name" variable passing the IOperation parameters
	* InvokeReturn(IOperation,"MethodName",IOperation[]): Using the IOperation variable
	* InvokeReturn&lt;Type>("MethodName",IOperation[]): Invoke a static method on type
	* InvokeReturn(Type,"MethodName",IOperation[]): Invoke a static method on type
* Set: (-) Invoke a setter on the variable
	* Set("name","Property",IOperation): Invoke the setter on property passing the IOperation as parameter
	* Set(IOperation,"Property",IOperation):Using the IOperation variable
* Get: (R) Invoke a getter on the variable
	* Get("name","Property"): Invoke the getter on property
	* Get(IOperation,"Property"):Using the IOperation variable

Just to give an example

<pre>
var newExpression = Function.Create()
		.WithParameter&lt;int>( "first")
		.WithParameter&lt;string>( "second")
		.WithBody(
			CodeLine.Assign("second", Operation.InvokeReturn("first", "ToString")),
			CodeLine.AssignConstant("second", "another", AssignementOperator.SumAssign)
		)
		.Returns("second")
		.ToExpression();
Assert.IsNotNull(newExpression);


var extMethodExpr = newExpression.Compile();
var result = extMethodExpr.DynamicInvoke(22, "b");
Assert.AreEqual("22another",result);
</pre>

##### Lambda operations
		
Lambda function can be invoked too

* Func: (R)
	Func<P1,R>( Func<P1,R>,IOperation): Calling a lambda with one parameter and a return variable
	Func<P1,P2,R>( Func<P1,P2,R>,IOperation,IOperation): Calling a lambda with two parameter and a return variable
* Action:
	Action<P1>( Action<P1>,IOperation): Calling a lambda with one parameter and a return variable
	Action<P1,P2>( Action<P1,P2>,IOperation,IOperation): Calling a lambda with two parameter and a return variable

Just a sample:

<pre>			
var newExpression = Function.Create()
	.WithParameter&lt;string>("first")
	.WithParameter&lt;string>("second")
	.WithBody(
			CodeLine.CreateVariable<string>( "result"),
			CodeLine.Assign("result",
				Operation.Func&lt;object, string, string>(
					//The lambda expression
					(a, b) =>
					{
						return string.Format("{0}-{1}",a,b);
					},
				Operation.Variable("first"),
				Operation.Variable("second"))
			)
	)
</pre>

##### Conditions

Condition are particular operation with return type of bool.

* Multiple conditions
	* And(Condition[]): (R) The and of the conditions listed
	* Or(Condition[]): (R) The or of the conditions listed

The single compare conditionst has the following operators

* Equal
* Different
* Greater: Does not work for strings
* GreaterEqual: Does not work for strings
* Smaller: Does not work for strings
* SmallerEqual: Does not work for strings
* ReferenceEqual	

* Comparaison operators
	* Compare("name1","name2",operator)
	* Compare(ILeftable,"name2",operator)
	* Compare("name1",IRightable,operator)
	* Compare(ILeftable,IRightable,operator)
	
##### If and While

If operator includes various kind of items.

* If(Condition): Verifiy a condition
* Then(CodeLine[])
* ElseIf(CodeLine[])
* Else(CodeLine[])

<pre>
var newExpression = Function.Create()
		.WithParameter&lt;int>("first")
		.WithBody(
				CodeLine.CreateIf(Condition.CompareConst("first", 3))
				.Then(CodeLine.AssignConstant("first", 2))
				.Else(CodeLine.AssignConstant("first", 1))
		)
</pre>

The while will be simpler, and kinda obvious...

* While(Condition)
* Do(CodeLine[])

### Utility classes

#### ReflectionUtil

A small utility class with utility methods through reflection

* string TypeToString(type): Return a string that describe the type including generics
* MethodCallDescriptor GetMethod(type, methodName,available paramTypes): Given a type, a method name and a list of types passed as parameters return the matching method info and the default values for the parameters not present into the paramTypes
* MethodCallDescriptor GetConstructor(Type type, List<Type> paramTypes): The same of GetMethod but for constructors

#### ExpressionUtil

A small utility class with utility methods through expressions

* IEnumerable&lt;LambdaPropertyDescriptor> GetPropertyInfos<TSource>(propertyLambda): Given a lambda expression for a property (or group of property) return the list of types/names of items present.
* Func&lt;TSource, object, bool> GetComparer<TSource>(propertyLambda, ComparaisonOperator): Given a lambda and a comparison type creates a lambda that compare an instance value with the passed value.

Some example will clarify :)

<pre>
    public class FirstLevel
    {
        public SecondLevel First { get; set; }
        public int Other { get; set; }
    }

    public class SecondLevel
    {
        public string Second { get; set; }
    }
	
	var type = ExpressionUtil.GetPropertyInfos&lt;FirstLevel>((a) => a.First.Second).ToArray();
	Assert.AreEqual(2, type.Length);
	Assert.AreEqual(typeof(string), type[1].DataType);
	Assert.AreEqual(typeof(SecondLevel), type[0].DataType);
	Assert.AreEqual("Second", type[1].Name);
	Assert.AreEqual("First", type[0].Name);
</pre>


### Download

See [kendar.org](http://www.kendar.org/?p=/dotnet/expressionsbuilder) for the latest changes.
